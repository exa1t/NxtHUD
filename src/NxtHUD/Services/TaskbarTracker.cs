using System.Windows.Threading;
using NxtHUD.Core.Flyouts;
using NxtHUD.Interop;

namespace NxtHUD.Services;

/// <summary>What the things NxtHUD puts on the taskbar need to know about it right now.</summary>
/// <param name="Handle">The primary taskbar window.</param>
/// <param name="Rect">Its bounds, in physical pixels.</param>
/// <param name="Scale">Its display's scale factor (1.0 at 100%).</param>
/// <param name="HasRoom">
/// False for a vertical taskbar, one slid away by auto-hide or one covered by a fullscreen app:
/// nothing should be shown on it then.
/// </param>
internal readonly record struct TaskbarState(IntPtr Handle, NativeMethods.RECT Rect, double Scale, bool HasRoom);

/// <summary>
/// Keeps track of the primary taskbar for the taskbar widget and the stats panel: re-checks it
/// on a slow timer, and scans where its own buttons are, so both can stay out of their way.
/// </summary>
/// <remarks>
/// The timer only runs while someone is showing something on the taskbar (see
/// <see cref="Acquire"/>). The button scan crosses into Explorer and takes tens of
/// milliseconds, so it runs on a worker thread; see <see cref="TaskbarButtonScanner"/>.
/// </remarks>
internal sealed class TaskbarTracker : IDisposable
{
    private const string TaskbarWindowClass = "Shell_TrayWnd";

    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _timer;
    private readonly HashSet<object> _users = new();
    private bool _scanning;

    public TaskbarTracker(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
        {
            Interval = TimeSpan.FromMilliseconds(1500),
        };
        _timer.Tick += OnTimerTick;
    }

    /// <summary>
    /// Raised on the UI thread every 1.5 seconds while in use, and as soon as the taskbar's
    /// buttons are found to have moved. Users re-place what they show.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>Where the taskbar's own buttons were at the last scan, in physical pixels; null until the first scan.</summary>
    public IReadOnlyList<TaskbarSpan>? Buttons { get; private set; }

    /// <summary>Starts tracking on behalf of <paramref name="user"/> (calling it again does nothing).</summary>
    public void Acquire(object user)
    {
        if (_users.Add(user) && _users.Count == 1)
        {
            _timer.Start();
            ScanButtons();
        }
    }

    /// <summary>Stops tracking for <paramref name="user"/>; the timer stops when nobody needs it.</summary>
    public void Release(object user)
    {
        if (_users.Remove(user) && _users.Count == 0)
        {
            _timer.Stop();
        }
    }

    /// <summary>Reads the primary taskbar's current state, or null when there is no taskbar (Explorer restarting).</summary>
    public static TaskbarState? GetState()
    {
        var taskbar = NativeMethods.FindWindow(TaskbarWindowClass, null);
        if (taskbar == IntPtr.Zero || !NativeMethods.GetWindowRect(taskbar, out var rect))
        {
            return null;
        }

        var display = ScreenHelper.GetDisplayOfWindow(taskbar);
        var isHorizontal = rect.Width > rect.Height;
        var visibleHeight = Math.Min(rect.Bottom, display.Bounds.Bottom) - Math.Max(rect.Top, display.Bounds.Y);
        var isOnScreen = visibleHeight >= rect.Height / 2.0;
        var hasRoom = isHorizontal && isOnScreen && !FullscreenDetector.IsFullscreenAppActive();

        return new TaskbarState(taskbar, rect, display.Scale, hasRoom);
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _users.Clear();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        // While a fullscreen game or app covers the taskbar the widget and stats are hidden
        // anyway, so leave Explorer alone; the last known layout is kept for when it ends.
        if (!FullscreenDetector.IsFullscreenAppActive())
        {
            ScanButtons();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Re-reads where the taskbar's buttons are, and raises <see cref="Changed"/> if they moved.</summary>
    private void ScanButtons()
    {
        if (_scanning)
        {
            return;
        }

        var taskbar = NativeMethods.FindWindow(TaskbarWindowClass, null);
        if (taskbar == IntPtr.Zero || !NativeMethods.GetWindowRect(taskbar, out var rect))
        {
            return;
        }

        _scanning = true;
        Task.Run(() => TaskbarButtonScanner.Scan(taskbar, rect.Top, rect.Bottom, rect.Width))
            .ContinueWith(
                task => _dispatcher.InvokeAsync(() =>
                {
                    _scanning = false;

                    if (task.Exception is { } error)
                    {
                        Log.Error("Reading the taskbar buttons failed unexpectedly.", error.GetBaseException());
                        return;
                    }

                    // A failed scan keeps the last known layout rather than dropping the limit.
                    if (task.Result is { } buttons && (Buttons is null || !buttons.SequenceEqual(Buttons)))
                    {
                        Buttons = buttons;
                        if (_users.Count > 0)
                        {
                            Changed?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }),
                TaskScheduler.Default);
    }
}
