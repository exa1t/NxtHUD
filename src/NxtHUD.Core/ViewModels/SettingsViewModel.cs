using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NxtHUD.Core.Localization;
using NxtHUD.Core.Services;
using NxtHUD.Core.Settings;
using NxtHUD.Core.Updates;

namespace NxtHUD.Core.ViewModels;

/// <summary>
/// Backs the settings window. Every property reads from and writes straight through to
/// <see cref="ISettingsService"/>, so there is no "Apply" button and no second copy of the
/// state to keep in sync.
/// </summary>
public sealed class SettingsViewModel : ObservableObject, IDisposable
{
    private readonly ISettingsService _settings;
    private readonly IStartupService _startup;
    private readonly UpdateChecker? _updates;

    private bool _applyingOwnChange;
    private string? _errorMessage;

    public SettingsViewModel(
        ISettingsService settings,
        IStartupService startup,
        string appVersion = "",
        string settingsFilePath = "",
        UpdateChecker? updates = null,
        Action? openReleasePage = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(startup);

        _settings = settings;
        _startup = startup;
        _updates = updates;
        AppVersion = appVersion;
        SettingsFilePath = settingsFilePath;

        ResetToDefaultsCommand = new RelayCommand(ResetToDefaults);
        CheckForUpdatesNowCommand = new AsyncRelayCommand(CheckForUpdatesNowAsync, () => _updates is not null);
        OpenReleasePageCommand = new RelayCommand(() => openReleasePage?.Invoke());
        _settings.Changed += OnSettingsChanged;
        Localizer.Instance.PropertyChanged += OnLanguageChanged;
        if (_updates is not null)
        {
            _updates.PropertyChanged += OnUpdatesChanged;
        }
    }

    // ----- Updates -----

    public bool CheckForUpdates
    {
        get => _settings.Current.CheckForUpdates;
        set => Apply(s => s.CheckForUpdates = value);
    }

    public IAsyncRelayCommand CheckForUpdatesNowCommand { get; }

    /// <summary>Opens the latest release's page, where the installer is.</summary>
    public IRelayCommand OpenReleasePageCommand { get; }

    public bool IsUpdateAvailable => _updates?.Status == UpdateStatus.Available;

    /// <summary>One line under the update switch saying what the last check found.</summary>
    public string UpdateStatusText => _updates?.Status switch
    {
        UpdateStatus.Checking => Localizer.Instance.Get("About_Updates_Checking"),
        UpdateStatus.UpToDate => Localizer.Instance.Get("About_Updates_UpToDate"),
        UpdateStatus.Available => string.Format(
            Localizer.Instance.Culture,
            Localizer.Instance.Get("About_Updates_Available"),
            _updates.AvailableVersion),
        UpdateStatus.Failed => Localizer.Instance.Get("About_Updates_Failed"),
        _ => Localizer.Instance.Get("About_Updates_Description"),
    };

    private Task CheckForUpdatesNowAsync() => _updates?.CheckAsync() ?? Task.CompletedTask;

    private void OnUpdatesChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(UpdateStatusText));
        OnPropertyChanged(nameof(IsUpdateAvailable));
    }

    private void OnLanguageChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(UpdateStatusText));
    }

    // ----- Choices offered by the pickers -----

    public IReadOnlyList<string> Languages => Localizer.SupportedLanguages;

    public IReadOnlyList<AppTheme> Themes { get; } = Enum.GetValues<AppTheme>();

    public IReadOnlyList<BackdropKind> Backdrops { get; } = Enum.GetValues<BackdropKind>();

    public IReadOnlyList<FlyoutVerticalPosition> VerticalPositions { get; } = Enum.GetValues<FlyoutVerticalPosition>();

    public IReadOnlyList<FlyoutHorizontalPosition> HorizontalPositions { get; } = Enum.GetValues<FlyoutHorizontalPosition>();

    public IReadOnlyList<AnimationSpeed> AnimationSpeeds { get; } = Enum.GetValues<AnimationSpeed>();

    public IReadOnlyList<FlyoutPosition> FlyoutPositions { get; } = Enum.GetValues<FlyoutPosition>();

    public IReadOnlyList<FlyoutDisplay> Displays { get; } = Enum.GetValues<FlyoutDisplay>();

    public IReadOnlyList<VisualizerColorMode> VisualizerColorModes { get; } = Enum.GetValues<VisualizerColorMode>();

    public IReadOnlyList<VisualizerAlignment> VisualizerAlignments { get; } = Enum.GetValues<VisualizerAlignment>();

    public double MaxTaskbarWidgetOffset => AppSettings.MaxTaskbarWidgetOffset;

    public double MinVisualizerBars => AppSettings.MinVisualizerBars;

    public double MaxVisualizerBars => AppSettings.MaxVisualizerBars;

    public double MinTimeoutSeconds => AppSettings.MinTimeoutMs / 1000.0;

    public double MaxTimeoutSeconds => AppSettings.MaxTimeoutMs / 1000.0;

    // ----- About -----

    public string AppVersion { get; }

    public string SettingsFilePath { get; }

    public IRelayCommand ResetToDefaultsCommand { get; }

    /// <summary>The last error worth showing to the user, or null.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(_errorMessage);

    // ----- General -----

    public bool StartWithWindows
    {
        get => _settings.Current.StartWithWindows;
        set
        {
            if (value == _settings.Current.StartWithWindows)
            {
                return;
            }

            try
            {
                _startup.SetEnabled(value);
                ErrorMessage = null;
            }
            catch (Exception ex) when (IsExpectedStartupFailure(ex))
            {
                // Leave the stored setting alone and make the switch snap back.
                ErrorMessage = ex.Message;
                OnPropertyChanged();
                return;
            }

            Apply(s => s.StartWithWindows = value);
        }
    }

    public string Language
    {
        get => _settings.Current.Language;
        set
        {
            if (value is null)
            {
                // A picker reports null while its item list is being rebuilt; ignore it.
                return;
            }

            Apply(s => s.Language = value);
        }
    }

    public bool DisableInFullscreen
    {
        get => _settings.Current.DisableInFullscreen;
        set => Apply(s => s.DisableInFullscreen = value);
    }

    // ----- Appearance -----

    public AppTheme Theme
    {
        get => _settings.Current.Theme;
        set => Apply(s => s.Theme = value);
    }

    public BackdropKind Backdrop
    {
        get => _settings.Current.Backdrop;
        set
        {
            Apply(s => s.Backdrop = value);
            OnPropertyChanged(nameof(IsTranslucentBackdrop));
        }
    }

    /// <summary>The opacity slider is only used by the Translucent backdrop.</summary>
    public bool IsTranslucentBackdrop => _settings.Current.Backdrop == BackdropKind.Translucent;

    public double MinTranslucentOpacity => AppSettings.MinTranslucentOpacity;

    public double MaxTranslucentOpacity => AppSettings.MaxTranslucentOpacity;

    /// <summary>Background opacity in percent; a slider supplies it as a double.</summary>
    public double TranslucentOpacity
    {
        get => _settings.Current.TranslucentOpacity;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                OnPropertyChanged();
                return;
            }

            var percent = (int)Math.Round(
                Math.Clamp(value, AppSettings.MinTranslucentOpacity, AppSettings.MaxTranslucentOpacity),
                MidpointRounding.AwayFromZero);
            Apply(s => s.TranslucentOpacity = percent);
        }
    }

    public FlyoutVerticalPosition VerticalPosition
    {
        get => _settings.Current.VerticalPosition;
        set => Apply(s => s.VerticalPosition = value);
    }

    public FlyoutHorizontalPosition HorizontalPosition
    {
        get => _settings.Current.HorizontalPosition;
        set => Apply(s => s.HorizontalPosition = value);
    }

    public bool CompactLayout
    {
        get => _settings.Current.CompactLayout;
        set => Apply(s => s.CompactLayout = value);
    }

    public AnimationSpeed AnimationSpeed
    {
        get => _settings.Current.AnimationSpeed;
        set => Apply(s => s.AnimationSpeed = value);
    }

    public FlyoutDisplay Display
    {
        get => _settings.Current.Display;
        set => Apply(s => s.Display = value);
    }

    /// <summary>The flyout timeout in seconds; stored in milliseconds and clamped to the allowed range.</summary>
    public double TimeoutSeconds
    {
        get => _settings.Current.FlyoutTimeoutMs / 1000.0;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                OnPropertyChanged();
                return;
            }

            var clampedSeconds = Math.Clamp(value, MinTimeoutSeconds, MaxTimeoutSeconds);
            var milliseconds = (int)Math.Round(clampedSeconds * 1000.0, MidpointRounding.AwayFromZero);
            Apply(s => s.FlyoutTimeoutMs = milliseconds);
        }
    }

    // ----- Flyouts -----

    public bool MediaFlyoutEnabled
    {
        get => _settings.Current.MediaFlyoutEnabled;
        set => Apply(s => s.MediaFlyoutEnabled = value);
    }

    public bool VolumeFlyoutEnabled
    {
        get => _settings.Current.VolumeFlyoutEnabled;
        set => Apply(s => s.VolumeFlyoutEnabled = value);
    }

    public bool UpNextFlyoutEnabled
    {
        get => _settings.Current.UpNextFlyoutEnabled;
        set => Apply(s => s.UpNextFlyoutEnabled = value);
    }

    public bool LockKeysFlyoutEnabled
    {
        get => _settings.Current.LockKeysFlyoutEnabled;
        set => Apply(s => s.LockKeysFlyoutEnabled = value);
    }

    public bool TaskbarWidgetEnabled
    {
        get => _settings.Current.TaskbarWidgetEnabled;
        set => Apply(s => s.TaskbarWidgetEnabled = value);
    }

    public FlyoutPosition VolumePosition
    {
        get => _settings.Current.VolumePosition;
        set => Apply(s => s.VolumePosition = value);
    }

    public FlyoutPosition LockKeysPosition
    {
        get => _settings.Current.LockKeysPosition;
        set => Apply(s => s.LockKeysPosition = value);
    }

    public FlyoutPosition UpNextPosition
    {
        get => _settings.Current.UpNextPosition;
        set => Apply(s => s.UpNextPosition = value);
    }

    public bool ReplaceNativeVolumeOverlay
    {
        get => _settings.Current.ReplaceNativeVolumeOverlay;
        set => Apply(s => s.ReplaceNativeVolumeOverlay = value);
    }

    /// <summary>Distance of the taskbar widget from the left end of the taskbar; stored as a whole number.</summary>
    public double TaskbarWidgetOffset
    {
        get => _settings.Current.TaskbarWidgetOffset;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                OnPropertyChanged();
                return;
            }

            var offset = (int)Math.Round(Math.Clamp(value, 0, AppSettings.MaxTaskbarWidgetOffset), MidpointRounding.AwayFromZero);
            Apply(s => s.TaskbarWidgetOffset = offset);
        }
    }

    public bool TaskbarVisualizerEnabled
    {
        get => _settings.Current.TaskbarVisualizerEnabled;
        set => Apply(s => s.TaskbarVisualizerEnabled = value);
    }

    /// <summary>Number of visualizer bars; a slider supplies it as a double.</summary>
    public double VisualizerBarCount
    {
        get => _settings.Current.VisualizerBarCount;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                OnPropertyChanged();
                return;
            }

            var count = (int)Math.Round(
                Math.Clamp(value, AppSettings.MinVisualizerBars, AppSettings.MaxVisualizerBars),
                MidpointRounding.AwayFromZero);
            Apply(s => s.VisualizerBarCount = count);
        }
    }

    public VisualizerColorMode VisualizerColorMode
    {
        get => _settings.Current.VisualizerColorMode;
        set
        {
            Apply(s => s.VisualizerColorMode = value);
            OnPropertyChanged(nameof(IsCustomVisualizerColor));
        }
    }

    public VisualizerAlignment VisualizerAlignment
    {
        get => _settings.Current.VisualizerAlignment;
        set => Apply(s => s.VisualizerAlignment = value);
    }

    /// <summary>The custom colour box is only editable when the Custom mode is picked.</summary>
    public bool IsCustomVisualizerColor => _settings.Current.VisualizerColorMode == VisualizerColorMode.Custom;

    /// <summary>"#RRGGBB" or "#AARRGGBB"; anything else is rejected and the box shows the stored value again.</summary>
    public string VisualizerCustomColor
    {
        get => _settings.Current.VisualizerCustomColor;
        set
        {
            if (!AppSettings.IsValidHexColor(value))
            {
                OnPropertyChanged();
                return;
            }

            var color = value.Trim();
            Apply(s => s.VisualizerCustomColor = color);
        }
    }

    public void Dispose()
    {
        _settings.Changed -= OnSettingsChanged;
        Localizer.Instance.PropertyChanged -= OnLanguageChanged;
        if (_updates is not null)
        {
            _updates.PropertyChanged -= OnUpdatesChanged;
        }
    }

    /// <summary>
    /// Writes one change through to the settings service and announces the property.
    /// The notification is raised even when the stored value did not change, so a control
    /// showing an out-of-range value snaps back to what was actually stored.
    /// </summary>
    private void Apply(Action<AppSettings> mutate, [CallerMemberName] string? propertyName = null)
    {
        _applyingOwnChange = true;
        try
        {
            _settings.Update(mutate);
        }
        finally
        {
            _applyingOwnChange = false;
        }

        OnPropertyChanged(propertyName);
    }

    private void ResetToDefaults()
    {
        ErrorMessage = null;

        try
        {
            _startup.SetEnabled(false);
        }
        catch (Exception ex) when (IsExpectedStartupFailure(ex))
        {
            ErrorMessage = ex.Message;
        }

        // Not an "own change": OnSettingsChanged refreshes every bound property.
        _settings.Replace(new AppSettings());
    }

    // ----- Taskbar stats -----

    public bool TaskbarStatsEnabled
    {
        get => _settings.Current.TaskbarStatsEnabled;
        set => Apply(s => s.TaskbarStatsEnabled = value);
    }

    public bool StatsShowCpu
    {
        get => _settings.Current.StatsShowCpu;
        set => Apply(s => s.StatsShowCpu = value);
    }

    public bool StatsShowRam
    {
        get => _settings.Current.StatsShowRam;
        set => Apply(s => s.StatsShowRam = value);
    }

    public bool StatsShowGpu
    {
        get => _settings.Current.StatsShowGpu;
        set => Apply(s => s.StatsShowGpu = value);
    }

    public bool StatsShowVram
    {
        get => _settings.Current.StatsShowVram;
        set => Apply(s => s.StatsShowVram = value);
    }

    public bool StatsShowGpuTemperature
    {
        get => _settings.Current.StatsShowGpuTemperature;
        set => Apply(s => s.StatsShowGpuTemperature = value);
    }

    public bool StatsShowNetwork
    {
        get => _settings.Current.StatsShowNetwork;
        set => Apply(s => s.StatsShowNetwork = value);
    }

    public bool StatsShowDisk
    {
        get => _settings.Current.StatsShowDisk;
        set => Apply(s => s.StatsShowDisk = value);
    }

    public double MinStatsIntervalSeconds => AppSettings.MinStatsIntervalMs / 1000.0;

    public double MaxStatsIntervalSeconds => AppSettings.MaxStatsIntervalMs / 1000.0;

    /// <summary>How often the taskbar stats refresh, in seconds; a slider supplies it in half-second steps.</summary>
    public double StatsIntervalSeconds
    {
        get => _settings.Current.StatsIntervalMs / 1000.0;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                OnPropertyChanged();
                return;
            }

            var ms = (int)Math.Round(
                Math.Clamp(value * 1000, AppSettings.MinStatsIntervalMs, AppSettings.MaxStatsIntervalMs),
                MidpointRounding.AwayFromZero);
            Apply(s => s.StatsIntervalMs = ms);
        }
    }

    private void OnSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (_applyingOwnChange)
        {
            return;
        }

        // An empty property name tells bindings that every property may have changed.
        OnPropertyChanged(string.Empty);
    }

    private static bool IsExpectedStartupFailure(Exception ex)
    {
        return ex is UnauthorizedAccessException
            or System.Security.SecurityException
            or IOException
            or InvalidOperationException;
    }
}
