using System.ComponentModel;
using System.IO;
using System.Windows;
using NxtHUD.Core.Flyouts;
using NxtHUD.Core.Input;
using NxtHUD.Core.Localization;
using NxtHUD.Core.Media;
using NxtHUD.Core.Migration;
using NxtHUD.Core.Settings;
using NxtHUD.Core.ViewModels;
using NxtHUD.Interop;
using NxtHUD.Services;
using NxtHUD.Views;

namespace NxtHUD;

/// <summary>
/// The composition root: creates the app's long-lived services, wires them together and
/// tears them down again. There is deliberately no DI container; a handful of objects with
/// explicit lifetimes keeps start-up fast and memory low for something that sits in the tray.
/// </summary>
internal sealed class AppHost : IDisposable
{
    /// <summary>First Windows build whose window manager offers Mica and Acrylic to any window (11 22H2).</summary>
    private const int FirstBuildWithSystemBackdrops = 22621;

    /// <summary>How long Windows needs to apply a key press before its effect can be read back.</summary>
    private static readonly TimeSpan KeySettleDelay = TimeSpan.FromMilliseconds(50);

    private readonly Application _app;
    private readonly bool _startedAtSignIn;
    private readonly FlyoutCoordinator _coordinator;
    private readonly FlyoutPresenter[] _presenters;
    private SettingsWindow? _settingsWindow;
    private bool _disposed;

    public AppHost(Application app, IReadOnlyList<string> args)
    {
        _app = app;
        _startedAtSignIn = args.Contains(AppInfo.AutostartArgument, StringComparer.OrdinalIgnoreCase);

        var dispatcher = app.Dispatcher;

        Settings = new SettingsService(AppInfo.SettingsPath);
        Startup = new StartupService(AppInfo.Name, AppInfo.GetAutostartCommand());
        Theme = new ThemeService(Settings, dispatcher);
        Tray = new TrayIconService();
        SettingsViewModel = new SettingsViewModel(Settings, Startup, AppInfo.Version, AppInfo.SettingsPath);

        Media = new SmtcMediaService(dispatcher);
        Volume = new CoreAudioVolumeService();
        KeyboardHook = new KeyboardHookService(dispatcher);

        Func<bool> isFullscreen = FullscreenDetector.IsFullscreenAppActive;

        MediaFlyout = new MediaFlyoutViewModel(Media, Settings, new DispatcherDelayTimer(dispatcher), null, isFullscreen);
        VolumeFlyout = new VolumeFlyoutViewModel(Volume, Settings, new DispatcherDelayTimer(dispatcher), isFullscreen);
        LockKeysFlyout = new LockKeysFlyoutViewModel(
            IsLockKeyOn,
            Settings,
            new DispatcherDelayTimer(dispatcher),
            Localizer.Instance,
            isFullscreen);
        UpNextFlyout = new UpNextFlyoutViewModel(
            Media,
            Settings,
            new DispatcherDelayTimer(dispatcher),
            () => MediaFlyout.IsVisible,
            isFullscreen);

        _coordinator = new FlyoutCoordinator(MediaFlyout, VolumeFlyout, LockKeysFlyout, UpNextFlyout);
        _presenters = new[]
        {
            new FlyoutPresenter(MediaFlyout, () => new MediaFlyoutWindow(MediaFlyout, CreateFlyoutContext())),
            new FlyoutPresenter(VolumeFlyout, () => new VolumeFlyoutWindow(VolumeFlyout, CreateFlyoutContext())),
            new FlyoutPresenter(LockKeysFlyout, () => new LockKeysFlyoutWindow(LockKeysFlyout, CreateFlyoutContext())),
            new FlyoutPresenter(UpNextFlyout, () => new UpNextFlyoutWindow(UpNextFlyout, CreateFlyoutContext())),
        };

        Spectrum = new LoopbackSpectrumSource(dispatcher);
        TaskbarWidget = new TaskbarWidgetViewModel(Media, Settings, MediaFlyout.Toggle, Spectrum);
        TaskbarTracker = new TaskbarTracker(dispatcher);
        TaskbarWidgetHost = new TaskbarWidgetService(TaskbarWidget, Settings, TaskbarTracker);

        Stats = new SystemStatsSource(dispatcher);
        TaskbarStats = new TaskbarStatsViewModel(Settings, Stats, OpenTaskManager);
        TaskbarStatsHost = new TaskbarStatsService(TaskbarStats, TaskbarTracker, TaskbarWidgetHost, dispatcher);
        DisplayWake = new DisplayWakeWatcher(dispatcher);
    }

    public SettingsService Settings { get; }

    public StartupService Startup { get; }

    public ThemeService Theme { get; }

    public TrayIconService Tray { get; }

    public SettingsViewModel SettingsViewModel { get; }

    public SmtcMediaService Media { get; }

    public CoreAudioVolumeService Volume { get; }

    public KeyboardHookService KeyboardHook { get; }

    public MediaFlyoutViewModel MediaFlyout { get; }

    public VolumeFlyoutViewModel VolumeFlyout { get; }

    public LockKeysFlyoutViewModel LockKeysFlyout { get; }

    public UpNextFlyoutViewModel UpNextFlyout { get; }

    /// <summary>Audio levels for the taskbar visualizer.</summary>
    internal LoopbackSpectrumSource Spectrum { get; }

    public TaskbarWidgetViewModel TaskbarWidget { get; }

    public TaskbarWidgetService TaskbarWidgetHost { get; }

    /// <summary>Follows the taskbar and its buttons for the widget and the stats panel.</summary>
    internal TaskbarTracker TaskbarTracker { get; }

    /// <summary>CPU, memory, GPU, network and disk figures for the stats panel.</summary>
    internal SystemStatsSource Stats { get; }

    public TaskbarStatsViewModel TaskbarStats { get; }

    internal TaskbarStatsService TaskbarStatsHost { get; }

    /// <summary>Asks the taskbar windows to rebuild after waking from sleep or a display change.</summary>
    internal DisplayWakeWatcher DisplayWake { get; }

    public void Start()
    {
        // Before anything reads the settings or the Run key: bring a user of the old NxtFlyout across.
        MigrateFromNxtFlyout();

        var loadResult = Settings.Load();
        Settings.SaveFailed += OnSettingsSaveFailed;
        Log.Info($"{AppInfo.Name} {AppInfo.Version} starting on Windows build {Environment.OSVersion.Version.Build} (settings: {loadResult}).");

        Localizer.Instance.SetLanguage(Settings.Current.Language);
        SyncStartupRegistration();
        Theme.Start();

        Settings.Changed += OnSettingsChanged;
        Tray.OpenSettingsRequested += OnOpenSettingsRequested;
        Tray.QuitRequested += OnQuitRequested;
        Tray.Show();

        KeyboardHook.MediaKeyPressed += OnMediaKeyPressed;
        KeyboardHook.VolumeKeyPressed += OnVolumeKeyPressed;
        KeyboardHook.LockKeyPressed += OnLockKeyPressed;
        UpdateKeyboardHook();

        TaskbarWidgetHost.Start();
        TaskbarStatsHost.Start();
        DisplayWake.RebuildRequested += OnRebuildTaskbarWindows;
        DisplayWake.Start();
        _ = StartMediaAsync();

        if (loadResult == SettingsLoadResult.CreatedDefaults)
        {
            // First run: write the defaults so the file exists, and show the user where the app lives.
            TrySaveSettings();

            if (!_startedAtSignIn)
            {
                ShowSettings();
            }
        }
    }

    /// <summary>Opens the settings window, or brings it forward if it is already open.</summary>
    public void ShowSettings()
    {
        if (_settingsWindow is null)
        {
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Closed += OnSettingsWindowClosed;
            _app.MainWindow = _settingsWindow;

            // Once before showing, so the window opens with the right backdrop,
            // and once after, when it has a handle the light/dark frame can be applied to.
            Theme.ApplyTo(_settingsWindow);
            _settingsWindow.Show();
            Theme.ApplyTo(_settingsWindow);
        }

        if (_settingsWindow.WindowState == WindowState.Minimized)
        {
            _settingsWindow.WindowState = WindowState.Normal;
        }

        _settingsWindow.Activate();

        // Activate() alone can be refused when another app owns the foreground
        // (for example when a second launch asked for this); a topmost blip gets past that.
        _settingsWindow.Topmost = true;
        _settingsWindow.Topmost = false;
    }

    /// <summary>Writes pending settings changes now; failures are logged, not thrown.</summary>
    public void TryFlushSettings()
    {
        try
        {
            Settings.Flush();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log.Error("Saving settings failed.", ex);
        }
    }

    private void OnRebuildTaskbarWindows(object? sender, EventArgs e)
    {
        // Widget first: the stats panel places itself around the widget.
        TaskbarWidgetHost.Rebuild();
        TaskbarStatsHost.Rebuild();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        Settings.Changed -= OnSettingsChanged;
        Settings.SaveFailed -= OnSettingsSaveFailed;
        Tray.OpenSettingsRequested -= OnOpenSettingsRequested;
        Tray.QuitRequested -= OnQuitRequested;
        KeyboardHook.MediaKeyPressed -= OnMediaKeyPressed;
        KeyboardHook.VolumeKeyPressed -= OnVolumeKeyPressed;
        KeyboardHook.LockKeyPressed -= OnLockKeyPressed;

        // Unhook first: the volume keys must work normally again the moment the app stops handling them.
        KeyboardHook.Dispose();

        DisplayWake.RebuildRequested -= OnRebuildTaskbarWindows;
        DisplayWake.Dispose();
        TaskbarStatsHost.Dispose();
        TaskbarStats.Dispose();
        Stats.Dispose();
        TaskbarWidgetHost.Dispose();
        TaskbarWidget.Dispose();
        TaskbarTracker.Dispose();
        Spectrum.Dispose();

        foreach (var presenter in _presenters)
        {
            presenter.Dispose();
        }

        _coordinator.Dispose();

        if (_settingsWindow is not null)
        {
            _settingsWindow.Closed -= OnSettingsWindowClosed;
            _settingsWindow.Close();
            _settingsWindow = null;
        }

        MediaFlyout.Dispose();
        VolumeFlyout.Dispose();
        LockKeysFlyout.Dispose();
        UpNextFlyout.Dispose();
        Media.Dispose();

        Tray.Dispose();
        Theme.Dispose();
        SettingsViewModel.Dispose();

        // Flushes pending changes; a failure is reported through SaveFailed, which is already unhooked,
        // so flush explicitly first to get it into the log.
        TryFlushSettings();
        Settings.Dispose();
    }

    private void TrySaveSettings()
    {
        try
        {
            Settings.Save();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log.Error("Creating the settings file failed.", ex);
        }
    }

    /// <summary>
    /// The registry, not the settings file, is the truth for start-with-Windows: the user can
    /// switch the entry off in Task Manager while the app is not looking.
    /// </summary>
    /// <summary>
    /// The app used to be called NxtFlyout: copy its settings from %AppData%\NxtFlyout and move
    /// its start-with-Windows entry to the new name. Never stops the app from starting.
    /// </summary>
    private static void MigrateFromNxtFlyout()
    {
        try
        {
            var migration = new LegacyMigration(new MigrationFileSystem(), new StartupRegistry(), Log.Info);
            var legacyDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                LegacyMigration.LegacyName);

            migration.MigrateSettings(legacyDirectory, AppInfo.DataDirectory);
            migration.MigrateAutostart(AppInfo.Name, AppInfo.GetAutostartCommand());
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            // For example the path of the running executable is unknown.
            Log.Error("Migrating from NxtFlyout failed; continuing without it.", ex);
        }
    }

    private void SyncStartupRegistration()
    {
        try
        {
            var enabled = Startup.IsEnabled();
            if (enabled)
            {
                Startup.RefreshCommandIfRegistered();
            }

            if (enabled != Settings.Current.StartWithWindows)
            {
                Settings.Update(s => s.StartWithWindows = enabled);
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException or IOException)
        {
            Log.Error("Reading the start-with-Windows registration failed.", ex);
        }
    }

    // ----- Flyouts -----

    /// <summary>
    /// Whether flyouts are drawn on a real Mica/Acrylic material. That needs the backdrop
    /// setting to ask for one and a Windows version that can supply it; otherwise flyouts
    /// fall back to a solid themed card.
    /// </summary>
    private static bool ShouldUseSystemBackdrop(AppSettings settings)
    {
        return settings.Backdrop is BackdropKind.Mica or BackdropKind.Acrylic
            && Environment.OSVersion.Version.Build >= FirstBuildWithSystemBackdrops;
    }

    private FlyoutWindowContext CreateFlyoutContext()
    {
        return new FlyoutWindowContext(Settings, () => Theme.IsDark, ShouldUseSystemBackdrop(Settings.Current));
    }

    /// <summary>Connects to the system media controls. The app works without them, minus the media features.</summary>
    private async Task StartMediaAsync()
    {
        try
        {
            await Media.StartAsync();
        }
        catch (Exception ex)
        {
            Log.Error("Connecting to the system media controls failed.", ex);
        }
    }

    /// <summary>
    /// Installs the keyboard hook only while a flyout that needs it is switched on, and decides
    /// whether the app takes over the volume keys.
    /// </summary>
    private void UpdateKeyboardHook()
    {
        var settings = Settings.Current;
        var needed = settings.MediaFlyoutEnabled || settings.VolumeFlyoutEnabled || settings.LockKeysFlyoutEnabled;

        // Only take the volume keys away from Windows if the app can actually change the volume.
        KeyboardHook.SuppressVolumeKeys = settings.VolumeFlyoutEnabled
            && settings.ReplaceNativeVolumeOverlay
            && Volume.GetState() is not null;

        try
        {
            if (needed && !KeyboardHook.IsRunning)
            {
                KeyboardHook.Start();
            }
            else if (!needed && KeyboardHook.IsRunning)
            {
                KeyboardHook.Stop();
            }
        }
        catch (Win32Exception ex)
        {
            Log.Error("Installing the keyboard hook failed.", ex);
        }
    }

    private void OnMediaKeyPressed(object? sender, MediaKey key) => MediaFlyout.OnMediaKeyPressed(key);

    private async void OnVolumeKeyPressed(object? sender, VolumeKey key)
    {
        try
        {
            if (KeyboardHook.SuppressVolumeKeys)
            {
                // Windows never saw the key, so the app applies it.
                var applied = key switch
                {
                    VolumeKey.Up => Volume.StepUp(),
                    VolumeKey.Down => Volume.StepDown(),
                    _ => Volume.ToggleMute(),
                };

                if (!applied)
                {
                    // Never leave the user with dead volume keys: hand them back to Windows.
                    KeyboardHook.SuppressVolumeKeys = false;
                    Log.Error("Changing the volume failed; the volume keys were handed back to Windows.");
                }
            }
            else
            {
                await Task.Delay(KeySettleDelay);
            }

            if (!_disposed)
            {
                VolumeFlyout.OnVolumeKeyHandled();
            }
        }
        catch (Exception ex)
        {
            Log.Error("Handling a volume key failed.", ex);
        }
    }

    private async void OnLockKeyPressed(object? sender, LockKey key)
    {
        try
        {
            // The hook runs before Windows flips the key's state.
            await Task.Delay(KeySettleDelay);

            if (!_disposed)
            {
                LockKeysFlyout.OnLockKeyPressed(key);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Handling a lock key failed.", ex);
        }
    }

    /// <summary>The low bit of GetKeyState is the toggle state of a lock key.</summary>
    private static bool IsLockKeyOn(LockKey key)
    {
        return (NativeMethods.GetKeyState(SystemKeyMap.ToVirtualKey(key)) & 1) != 0;
    }

    // ----- Settings and tray -----

    private void OnSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (!_app.Dispatcher.CheckAccess())
        {
            _app.Dispatcher.InvokeAsync(() => OnSettingsChanged(sender, e));
            return;
        }

        var previous = e.Previous;
        var current = e.Current;

        if (previous.Language != current.Language)
        {
            Localizer.Instance.SetLanguage(current.Language);
        }

        if (previous.Theme != current.Theme || previous.Backdrop != current.Backdrop)
        {
            Theme.Apply();
        }

        if (ShouldUseSystemBackdrop(previous) != ShouldUseSystemBackdrop(current))
        {
            // Solid card versus system backdrop is fixed when a flyout window is created.
            foreach (var presenter in _presenters)
            {
                presenter.Reset();
            }
        }

        if (previous.MediaFlyoutEnabled != current.MediaFlyoutEnabled
            || previous.VolumeFlyoutEnabled != current.VolumeFlyoutEnabled
            || previous.LockKeysFlyoutEnabled != current.LockKeysFlyoutEnabled
            || previous.ReplaceNativeVolumeOverlay != current.ReplaceNativeVolumeOverlay)
        {
            UpdateKeyboardHook();
        }
    }

    private static void OnSettingsSaveFailed(object? sender, Exception exception)
    {
        Log.Error("Saving settings failed.", exception);
    }

    /// <summary>Clicking the taskbar stats opens Task Manager, where the full details are.</summary>
    private static void OpenTaskManager()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("taskmgr.exe") { UseShellExecute = true });
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            // For example Task Manager is disabled by policy.
            Log.Error("Opening Task Manager failed.", ex);
        }
    }

    private void OnOpenSettingsRequested(object? sender, EventArgs e) => ShowSettings();

    private void OnQuitRequested(object? sender, EventArgs e)
    {
        // Deferred so the tray menu can finish closing before the app tears itself down.
        _app.Dispatcher.InvokeAsync(() => _app.Shutdown());
    }

    private void OnSettingsWindowClosed(object? sender, EventArgs e)
    {
        if (_settingsWindow is not null)
        {
            if (ReferenceEquals(_app.MainWindow, _settingsWindow))
            {
                _app.MainWindow = null;
            }

            _settingsWindow.Closed -= OnSettingsWindowClosed;
            _settingsWindow = null;
        }

        TryFlushSettings();
    }
}
