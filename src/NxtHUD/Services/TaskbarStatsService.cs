using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Threading;
using NxtHUD.Core.Flyouts;
using NxtHUD.Core.ViewModels;
using NxtHUD.Interop;
using NxtHUD.Views;

namespace NxtHUD.Services;

/// <summary>
/// Shows the system stats panel on the primary taskbar, in the free space just left of the
/// tray (the notification icons and clock), and keeps it there.
/// </summary>
/// <remarks>
/// Like the taskbar widget it is a small window owned by the taskbar. It takes whatever room
/// is left between the app icons (or the widget) and the tray: two lines when they fit, one
/// line when they do not, and nothing when not even one line fits. The text changes width as
/// the figures change, so the panel is re-placed after every reading, keeping its right edge
/// against the tray.
/// </remarks>
internal sealed class TaskbarStatsService : IDisposable
{
    /// <summary>Gap kept clear on either side of the panel, in device-independent pixels.</summary>
    private const double Gap = 8;

    /// <summary>Space left above and below the panel inside the taskbar, in device-independent pixels.</summary>
    private const double VerticalInset = 2;

    private readonly TaskbarStatsViewModel _viewModel;
    private readonly TaskbarTracker _tracker;
    private readonly TaskbarWidgetService _widget;
    private readonly Dispatcher _dispatcher;

    private TaskbarStatsWindow? _window;
    private IntPtr _ownerTaskbar;
    private bool _started;
    private bool _updatePending;

    public TaskbarStatsService(TaskbarStatsViewModel viewModel, TaskbarTracker tracker, TaskbarWidgetService widget, Dispatcher dispatcher)
    {
        _viewModel = viewModel;
        _tracker = tracker;
        _widget = widget;
        _dispatcher = dispatcher;
    }

    public void Start()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _tracker.Changed += OnSomethingMoved;
        _widget.OccupiedSpanChanged += OnSomethingMoved;
        Update();
    }

    public void Dispose()
    {
        if (_started)
        {
            _started = false;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _tracker.Changed -= OnSomethingMoved;
            _widget.OccupiedSpanChanged -= OnSomethingMoved;
        }

        _tracker.Release(this);
        CloseWindow();
    }

    /// <summary>
    /// Replaces the panel's window with a fresh one. After the graphics are reset (waking from
    /// sleep) a see-through window can turn solid gray until it is recreated.
    /// </summary>
    public void Rebuild()
    {
        if (!_started || _window is null)
        {
            return;
        }

        CloseWindow();
        Update();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskbarStatsViewModel.IsVisible))
        {
            Update();
        }
    }

    private void OnSomethingMoved(object? sender, EventArgs e) => Update();

    /// <summary>
    /// Both lines change on every reading; re-place once for the pair rather than once per line.
    /// </summary>
    private void OnTextChanged(object? sender, EventArgs e)
    {
        if (_updatePending)
        {
            return;
        }

        _updatePending = true;
        _dispatcher.InvokeAsync(
            () =>
            {
                _updatePending = false;
                Update();
            },
            DispatcherPriority.Background);
    }

    private void Update()
    {
        if (!_started || !_viewModel.IsVisible)
        {
            _tracker.Release(this);
            CloseWindow();
            return;
        }

        _tracker.Acquire(this);

        try
        {
            Place();
        }
        catch (Exception ex) when (ex is InvalidOperationException or Win32Exception or ArgumentException)
        {
            Log.Error("Placing the taskbar stats failed.", ex);
            CloseWindow();
        }
    }

    private void Place()
    {
        if (TaskbarTracker.GetState() is not { } taskbar)
        {
            CloseWindow();
            return;
        }

        // Until the first scan says where the tray is, there is no telling where the free space ends.
        if (!taskbar.HasRoom || _tracker.Buttons is not { } buttons)
        {
            HideWindow();
            return;
        }

        if (_window is not null && _ownerTaskbar != taskbar.Handle)
        {
            CloseWindow();
        }

        if (_window is null)
        {
            _window = new TaskbarStatsWindow(_viewModel);
            _window.Closed += OnWindowClosed;
            _window.TextChanged += OnTextChanged;
            _ownerTaskbar = taskbar.Handle;
            new WindowInteropHelper(_window).Owner = taskbar.Handle;
        }

        var rect = taskbar.Rect;
        var scale = taskbar.Scale;
        var height = Math.Max(24, (rect.Height / scale) - (2 * VerticalInset));
        if (Math.Abs(_window.Height - height) > 0.5)
        {
            _window.Height = height;
        }

        _window.SetTaskbarTheme(ThemeService.IsTaskbarDark());

        // The widget is our own window, so the button scan does not see it; add it by hand.
        var obstacles = _widget.OccupiedSpan is { } widget ? buttons.Append(widget) : buttons;
        var span = TaskbarWidgetFit.FindFreeSpanBefore(rect.Left, rect.Right, rect.Right, obstacles, Gap * scale);
        if (span is null)
        {
            HideWindow();
            return;
        }

        var room = span.Value.Width / scale;
        var widthOf = _window.MeasureLayouts();
        var layout = TaskbarWidgetFit.ChooseLayout<TaskbarStatsLayout>(room, widthOf);
        if (layout is null)
        {
            HideWindow();
            return;
        }

        _window.ApplyLayout(layout.Value);
        var width = Math.Min(widthOf(layout.Value), room);
        if (Math.Abs(_window.MaxWidth - room) > 0.5)
        {
            _window.MaxWidth = room;
        }

        // Keep the right edge against the tray, however wide the text is this second.
        var x = span.Value.Right - (width * scale);
        var y = rect.Top + (VerticalInset * scale);
        NativeMethods.MoveWindowTo(new WindowInteropHelper(_window).EnsureHandle(), x, y);

        if (!_window.IsVisible)
        {
            _window.Show();
        }
    }

    private void HideWindow()
    {
        if (_window is not null && _window.IsVisible)
        {
            _window.Hide();
        }
    }

    private void CloseWindow()
    {
        var window = _window;
        if (window is null)
        {
            return;
        }

        _window = null;
        _ownerTaskbar = IntPtr.Zero;
        window.Closed -= OnWindowClosed;
        window.TextChanged -= OnTextChanged;
        window.Close();
    }

    /// <summary>Windows destroys an owned window together with its owner (an Explorer restart).</summary>
    private void OnWindowClosed(object? sender, EventArgs e)
    {
        if (_window is not null)
        {
            _window.Closed -= OnWindowClosed;
            _window.TextChanged -= OnTextChanged;
            _window = null;
            _ownerTaskbar = IntPtr.Zero;
        }
    }
}
