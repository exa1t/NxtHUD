using System.ComponentModel;
using System.Windows.Interop;
using NxtHUD.Core.Flyouts;
using NxtHUD.Core.Settings;
using NxtHUD.Core.ViewModels;
using NxtHUD.Interop;
using NxtHUD.Views;

namespace NxtHUD.Services;

/// <summary>
/// Shows the now-playing widget on the primary taskbar while the view model says it should
/// be visible, and keeps it in place.
/// </summary>
/// <remarks>
/// <para>The widget is a separate small window owned by the taskbar window. Windows always
/// keeps an owned window above its owner, which is what keeps the widget from disappearing
/// behind the taskbar when the taskbar is clicked.</para>
/// <para>Windows offers no API for adding content to the taskbar. <see cref="TaskbarTracker"/>
/// re-checks the taskbar on a slow timer (it moves with display changes, auto-hide and Explorer
/// restarts) and finds where its own buttons are; the widget takes the free space at the left
/// and hides while a fullscreen app covers the taskbar.</para>
/// </remarks>
internal sealed class TaskbarWidgetService : IDisposable
{
    /// <summary>Gap between the widget and the left end of the taskbar, in device-independent pixels.</summary>
    private const double LeftGap = 8;

    /// <summary>Space left above and below the widget inside the taskbar, in device-independent pixels.</summary>
    private const double VerticalInset = 4;

    /// <summary>Widest the widget gets even on an empty taskbar, in device-independent pixels.</summary>
    private const double MaxWidth = 480;

    private readonly TaskbarWidgetViewModel _viewModel;
    private readonly ISettingsService _settings;
    private readonly TaskbarTracker _tracker;

    private TaskbarWidgetWindow? _window;
    private IntPtr _ownerTaskbar;
    private bool _started;
    private TaskbarSpan? _occupiedSpan;

    public TaskbarWidgetService(TaskbarWidgetViewModel viewModel, ISettingsService settings, TaskbarTracker tracker)
    {
        _viewModel = viewModel;
        _settings = settings;
        _tracker = tracker;
    }

    /// <summary>Raised when <see cref="OccupiedSpan"/> changes.</summary>
    public event EventHandler? OccupiedSpanChanged;

    /// <summary>The stretch of the taskbar the widget covers, in physical pixels, or null while it is hidden.</summary>
    public TaskbarSpan? OccupiedSpan
    {
        get => _occupiedSpan;
        private set
        {
            if (_occupiedSpan != value)
            {
                _occupiedSpan = value;
                OccupiedSpanChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void Start()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _settings.Changed += OnSettingsChanged;
        _tracker.Changed += OnTaskbarChanged;
        Update();
    }

    public void Dispose()
    {
        if (_started)
        {
            _started = false;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _settings.Changed -= OnSettingsChanged;
            _tracker.Changed -= OnTaskbarChanged;
        }

        _tracker.Release(this);
        CloseWindow();
    }

    /// <summary>
    /// Replaces the widget's window with a fresh one. After the graphics are reset (waking from
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
        switch (e.PropertyName)
        {
            case nameof(TaskbarWidgetViewModel.IsVisible):
            // These change the widget's width, so the layout may have to step up or down.
            case nameof(TaskbarWidgetViewModel.Title):
            case nameof(TaskbarWidgetViewModel.Artist):
            case nameof(TaskbarWidgetViewModel.ShowVisualizer):
            case nameof(TaskbarWidgetViewModel.VisualizerBarCount):
                Update();
                break;
        }
    }

    private void OnSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.Previous.TaskbarWidgetOffset != e.Current.TaskbarWidgetOffset)
        {
            Update();
        }
    }

    private void OnTaskbarChanged(object? sender, EventArgs e) => Update();

    /// <summary>Brings the widget in line with the view model and the taskbar's current state.</summary>
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
            // For example the taskbar window vanished mid-way because Explorer restarted.
            Log.Error("Placing the taskbar widget failed.", ex);
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

        if (!taskbar.HasRoom)
        {
            HideWindow();
            return;
        }

        if (_window is not null && _ownerTaskbar != taskbar.Handle)
        {
            // Explorer restarted and made a new taskbar; the widget needs the new one as its owner.
            CloseWindow();
        }

        if (_window is null)
        {
            _window = new TaskbarWidgetWindow(_viewModel);
            _window.Closed += OnWindowClosed;
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

        var handle = new WindowInteropHelper(_window).EnsureHandle();
        var x = rect.Left + ((LeftGap + _settings.Current.TaskbarWidgetOffset) * scale);
        var y = rect.Top + (VerticalInset * scale);

        // Fit between the taskbar's own buttons when we know where they are. Until the first
        // scan comes back, or if the taskbar cannot be read, behave as before: no limit.
        var room = MaxWidth;
        if (_tracker.Buttons is { } buttons)
        {
            var span = TaskbarWidgetFit.FindFreeSpan(rect.Left, rect.Right, x, buttons, LeftGap * scale);
            if (span is null)
            {
                HideWindow();
                return;
            }

            x = span.Value.Left;
            room = Math.Min(MaxWidth, span.Value.Width / scale);
        }

        var widthOf = _window.MeasureLayouts();
        var layout = TaskbarWidgetFit.ChooseLayout(room, widthOf);
        if (layout is null)
        {
            HideWindow();
            return;
        }

        _window.ApplyLayout(layout.Value);

        // A hard limit as well, so a longer title arriving between two checks cannot spill over.
        if (Math.Abs(_window.MaxWidth - room) > 0.5)
        {
            _window.MaxWidth = room;
        }

        NativeMethods.MoveWindowTo(handle, x, y);

        if (!_window.IsVisible)
        {
            _window.Show();
        }

        OccupiedSpan = new TaskbarSpan(x, x + (Math.Min(widthOf(layout.Value), room) * scale));
    }

    private void HideWindow()
    {
        if (_window is not null && _window.IsVisible)
        {
            _window.Hide();
        }

        OccupiedSpan = null;
    }

    private void CloseWindow()
    {
        OccupiedSpan = null;

        var window = _window;
        if (window is null)
        {
            return;
        }

        _window = null;
        _ownerTaskbar = IntPtr.Zero;
        window.Closed -= OnWindowClosed;
        window.Close();
    }

    /// <summary>Windows destroys an owned window together with its owner (an Explorer restart).</summary>
    private void OnWindowClosed(object? sender, EventArgs e)
    {
        if (_window is not null)
        {
            _window.Closed -= OnWindowClosed;
            _window = null;
            _ownerTaskbar = IntPtr.Zero;
        }

        OccupiedSpan = null;
    }
}
