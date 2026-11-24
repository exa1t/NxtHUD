using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;

namespace NxtHUD.Services;

/// <summary>
/// Tells the taskbar windows when to rebuild themselves: after the screen comes back on from
/// sleep, and after the display setup changes.
/// </summary>
/// <remarks>
/// <para>The taskbar widget and the stats panel are see-through (layered) WPF windows. When the
/// graphics are reset, as on waking from Modern Standby, such a window can lose its see-through
/// surface: it keeps drawing its text, but its clear parts turn solid gray until it is
/// recreated. The reset can also land a few seconds after the screen is back, so
/// <see cref="RebuildRequested"/> is raised twice, shortly after and again a little later.</para>
/// <para>Modern Standby does not reliably send the classic resume broadcast, so this listens
/// for the console display turning on (GUID_CONSOLE_DISPLAY_STATE) as well.</para>
/// </remarks>
internal sealed class DisplayWakeWatcher : IDisposable
{
    private static readonly TimeSpan FirstRebuild = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan SecondRebuild = TimeSpan.FromSeconds(10);

    private static Guid _consoleDisplayState = new("6FE69556-704A-47A0-8F24-C28D936FDA47");

    private const int WM_POWERBROADCAST = 0x0218;
    private const int PBT_APMRESUMEAUTOMATIC = 0x0012;
    private const int PBT_POWERSETTINGCHANGE = 0x8013;
    private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _first;
    private readonly DispatcherTimer _second;

    private HwndSource? _source;
    private IntPtr _notification;
    private int? _displayState;

    public DisplayWakeWatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _first = CreateTimer(FirstRebuild);
        _second = CreateTimer(SecondRebuild);
    }

    /// <summary>Raised on the UI thread when the taskbar windows should be recreated.</summary>
    public event EventHandler? RebuildRequested;

    public void Start()
    {
        if (_source is not null)
        {
            return;
        }

        // A hidden window of our own to receive the power notifications. Not message-only:
        // those do not get WM_POWERBROADCAST.
        _source = new HwndSource(new HwndSourceParameters("NxtHUD display watcher")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0,
        });
        _source.AddHook(WndProc);

        _notification = RegisterPowerSettingNotification(_source.Handle, ref _consoleDisplayState, DEVICE_NOTIFY_WINDOW_HANDLE);
        if (_notification == IntPtr.Zero)
        {
            Log.Info($"Watching the display state failed (error {Marshal.GetLastWin32Error()}); relying on the resume broadcast.");
        }

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _first.Stop();
        _second.Stop();

        if (_notification != IntPtr.Zero)
        {
            UnregisterPowerSettingNotification(_notification);
            _notification = IntPtr.Zero;
        }

        if (_source is not null)
        {
            _source.RemoveHook(WndProc);
            _source.Dispose();
            _source = null;
        }
    }

    private DispatcherTimer CreateTimer(TimeSpan delay)
    {
        var timer = new DispatcherTimer(DispatcherPriority.Normal, _dispatcher) { Interval = delay };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            RebuildRequested?.Invoke(this, EventArgs.Empty);
        };
        return timer;
    }

    /// <summary>Starts (or restarts) both rebuild timers, so a burst of events gives one pair.</summary>
    private void ScheduleRebuild(string reason)
    {
        if (!_dispatcher.CheckAccess())
        {
            _dispatcher.InvokeAsync(() => ScheduleRebuild(reason));
            return;
        }

        Log.Info($"Rebuilding the taskbar windows: {reason}.");
        _first.Stop();
        _first.Start();
        _second.Stop();
        _second.Start();
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e) => ScheduleRebuild("the display settings changed");

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_POWERBROADCAST)
        {
            return IntPtr.Zero;
        }

        switch (wParam.ToInt32())
        {
            case PBT_APMRESUMEAUTOMATIC:
                ScheduleRebuild("the PC resumed");
                break;

            case PBT_POWERSETTINGCHANGE when lParam != IntPtr.Zero:
                var setting = Marshal.PtrToStructure<PowerBroadcastSetting>(lParam);
                if (setting.PowerSetting == _consoleDisplayState && setting.DataLength >= sizeof(int))
                {
                    // 0 = off, 1 = on, 2 = dimmed. Windows sends the current state right after
                    // registering; only coming back on from off or dimmed counts as waking
                    // (some PCs go dimmed -> on without reporting off in between).
                    var state = Marshal.ReadInt32(lParam, Marshal.OffsetOf<PowerBroadcastSetting>(nameof(PowerBroadcastSetting.Data)).ToInt32());
                    var previous = _displayState;
                    _displayState = state;
                    if (previous is 0 or 2 && state == 1)
                    {
                        ScheduleRebuild("the display turned back on");
                    }
                }

                break;
        }

        return IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PowerBroadcastSetting
    {
        public Guid PowerSetting;
        public int DataLength;
        public byte Data;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid powerSettingGuid, int flags);

    [DllImport("user32.dll")]
    private static extern bool UnregisterPowerSettingNotification(IntPtr handle);
}
