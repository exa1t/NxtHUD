using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using NxtHUD.Core.Localization;
using NxtHUD.Interop;
using WinForms = System.Windows.Forms;
using WpfUi = Wpf.Ui.Controls;

namespace NxtHUD.Services;

/// <summary>
/// The notification-area (tray) icon and its menu.
/// </summary>
/// <remarks>
/// The icon itself is a Windows Forms <see cref="WinForms.NotifyIcon"/>, but the menu is a
/// WPF <see cref="ContextMenu"/> so it picks up the Fluent styling, theme and accent colour.
/// </remarks>
internal sealed class TrayIconService : IDisposable
{
    private static readonly Uri IconUri = new("pack://application:,,,/Assets/NxtHUD.ico");

    private readonly System.Drawing.Icon _icon;
    private readonly WinForms.NotifyIcon _notifyIcon;
    private ContextMenu? _menu;
    private MenuItem? _updateItem;
    private Separator? _updateSeparator;
    private string? _updateVersion;
    private Window? _menuOwner;
    private bool _disposed;

    public TrayIconService()
    {
        _icon = LoadIcon();
        _notifyIcon = new WinForms.NotifyIcon
        {
            Icon = _icon,
            Text = AppInfo.Name,
            Visible = false,
        };
        _notifyIcon.MouseUp += OnMouseUp;
        _notifyIcon.BalloonTipClicked += OnBalloonTipClicked;
    }

    /// <summary>The user clicked the icon or chose "Settings".</summary>
    public event EventHandler? OpenSettingsRequested;

    /// <summary>The user chose "Quit".</summary>
    public event EventHandler? QuitRequested;

    /// <summary>The user clicked the update notification or the update menu item.</summary>
    public event EventHandler? UpdateRequested;

    public void Show() => _notifyIcon.Visible = true;

    /// <summary>
    /// Adds a "Download NxtHUD x" item to the menu while an update is available, or removes it
    /// (null).
    /// </summary>
    public void SetAvailableUpdate(string? version)
    {
        _updateVersion = version;
        if (_menu is not null && _updateItem is not null)
        {
            UpdateMenuItemState();
        }
    }

    /// <summary>Shows a Windows notification saying a new version is out; clicking it raises <see cref="UpdateRequested"/>.</summary>
    public void ShowUpdateNotice(string version)
    {
        var localizer = Localizer.Instance;
        _notifyIcon.ShowBalloonTip(
            10_000,
            string.Format(localizer.Culture, localizer.Get("Tray_UpdateAvailable_Title"), version),
            localizer.Get("Tray_UpdateAvailable_Text"),
            WinForms.ToolTipIcon.Info);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _notifyIcon.MouseUp -= OnMouseUp;
        _notifyIcon.BalloonTipClicked -= OnBalloonTipClicked;

        // Hide before disposing, otherwise a ghost icon lingers until the mouse moves over it.
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _icon.Dispose();

        if (_menu is not null)
        {
            _menu.IsOpen = false;
        }

        _menuOwner?.Close();
    }

    private static System.Drawing.Icon LoadIcon()
    {
        var resource = Application.GetResourceStream(IconUri)
            ?? throw new InvalidOperationException("The tray icon resource is missing.");

        using var stream = resource.Stream;

        // Picks the frame closest to the tray icon size for the current DPI.
        return new System.Drawing.Icon(stream, WinForms.SystemInformation.SmallIconSize);
    }

    private void OnMouseUp(object? sender, WinForms.MouseEventArgs e)
    {
        switch (e.Button)
        {
            case WinForms.MouseButtons.Left:
                OpenSettingsRequested?.Invoke(this, EventArgs.Empty);
                break;

            case WinForms.MouseButtons.Right:
                ShowMenu();
                break;
        }
    }

    private void ShowMenu()
    {
        _menu ??= BuildMenu();
        UpdateMenuItemState();
        _menuOwner ??= CreateMenuOwner();

        // A popup only closes when the user clicks elsewhere if its thread owns the foreground
        // window, so bring an invisible helper window to the foreground first.
        NativeMethods.SetForegroundWindow(new WindowInteropHelper(_menuOwner).Handle);

        _menu.IsOpen = true;
    }

    private ContextMenu BuildMenu()
    {
        var settingsItem = CreateMenuItem("Tray_Settings", WpfUi.SymbolRegular.Settings24);
        settingsItem.Click += (_, _) => OpenSettingsRequested?.Invoke(this, EventArgs.Empty);

        var quitItem = CreateMenuItem("Tray_Quit", WpfUi.SymbolRegular.Dismiss24);
        quitItem.Click += (_, _) => QuitRequested?.Invoke(this, EventArgs.Empty);

        _updateItem = new MenuItem
        {
            Icon = new WpfUi.SymbolIcon { Symbol = WpfUi.SymbolRegular.ArrowDownload24 },
        };
        _updateItem.Click += (_, _) => UpdateRequested?.Invoke(this, EventArgs.Empty);
        _updateSeparator = new Separator();

        var menu = new ContextMenu
        {
            Placement = PlacementMode.MousePoint,
        };
        menu.Items.Add(_updateItem);
        menu.Items.Add(_updateSeparator);
        menu.Items.Add(settingsItem);
        menu.Items.Add(new Separator());
        menu.Items.Add(quitItem);
        _menu = menu;
        UpdateMenuItemState();
        return menu;
    }

    private void UpdateMenuItemState()
    {
        var visibility = _updateVersion is null ? Visibility.Collapsed : Visibility.Visible;
        _updateItem!.Visibility = visibility;
        _updateSeparator!.Visibility = visibility;
        if (_updateVersion is not null)
        {
            var localizer = Localizer.Instance;
            _updateItem.Header = string.Format(localizer.Culture, localizer.Get("Tray_Update"), _updateVersion);
        }
    }

    private void OnBalloonTipClicked(object? sender, EventArgs e) => UpdateRequested?.Invoke(this, EventArgs.Empty);

    private static MenuItem CreateMenuItem(string textKey, WpfUi.SymbolRegular symbol)
    {
        var item = new MenuItem
        {
            Icon = new WpfUi.SymbolIcon { Symbol = symbol },
        };

        // Bound rather than assigned, so the text follows a language change.
        item.SetBinding(
            HeaderedItemsControl.HeaderProperty,
            new Binding($"[{textKey}]") { Source = Localizer.Instance, Mode = BindingMode.OneWay });

        return item;
    }

    /// <summary>Creates a window that is never shown; it only lends its handle to the menu.</summary>
    private Window CreateMenuOwner()
    {
        var window = new Window
        {
            Title = AppInfo.Name,
            Width = 0,
            Height = 0,
            Left = -32000,
            Top = -32000,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            ShowActivated = false,
        };

        // WPF closes every window at shutdown; forget this one then so Dispose does not close it twice.
        window.Closed += (_, _) => _menuOwner = null;

        new WindowInteropHelper(window).EnsureHandle();

        // WPF treats the first window created as the app's main window. This helper must not
        // take that role, or theme updates aimed at the main window would land on it.
        if (ReferenceEquals(Application.Current.MainWindow, window))
        {
            Application.Current.MainWindow = null;
        }

        return window;
    }
}
