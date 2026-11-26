using System.Runtime.InteropServices;

namespace NxtHUD.Interop;

/// <summary>Win32 functions and constants used by the app.</summary>
internal static class NativeMethods
{
    // ----- Low-level keyboard hook -----

    internal const int WH_KEYBOARD_LL = 13;
    internal const int WM_KEYDOWN = 0x0100;
    internal const int WM_KEYUP = 0x0101;
    internal const int WM_SYSKEYDOWN = 0x0104;
    internal const int WM_SYSKEYUP = 0x0105;

    // ----- Window styles and positioning -----

    internal const int GWL_EXSTYLE = -20;
    internal const long WS_EX_TOOLWINDOW = 0x00000080L;
    internal const long WS_EX_NOACTIVATE = 0x08000000L;

    internal const uint SWP_NOSIZE = 0x0001;
    internal const uint SWP_NOMOVE = 0x0002;
    internal const uint SWP_NOZORDER = 0x0004;
    internal const uint SWP_NOACTIVATE = 0x0010;

    internal const uint MONITOR_DEFAULTTOPRIMARY = 1;
    internal const uint MONITOR_DEFAULTTONEAREST = 2;

    /// <summary>MDT_EFFECTIVE_DPI for GetDpiForMonitor.</summary>
    internal const int MDT_EFFECTIVE_DPI = 0;

    // ----- Desktop Window Manager attributes -----

    internal const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    internal const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    internal const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

    internal const int DWMWCP_ROUND = 2;

    /// <summary>DWMSBT_MAINWINDOW: Mica.</summary>
    internal const int DWMSBT_MICA = 2;

    /// <summary>DWMSBT_TRANSIENTWINDOW: Acrylic.</summary>
    internal const int DWMSBT_ACRYLIC = 3;

    // ----- SHQueryUserNotificationState results -----

    internal const int QUNS_BUSY = 2;
    internal const int QUNS_RUNNING_D3D_FULL_SCREEN = 3;

    /// <summary>Callback for a low-level keyboard hook.</summary>
    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public readonly int Width => Right - Left;

        public readonly int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    internal static extern short GetKeyState(int nVirtKey);

    [DllImport("shell32.dll")]
    internal static extern int SHQueryUserNotificationState(out int state);

    [DllImport("user32.dll", EntryPoint = "FindWindowW", CharSet = CharSet.Unicode)]
    internal static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("shcore.dll")]
    internal static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    internal const int WM_NCACTIVATE = 0x0086;

    private const int GWL_STYLE = -16;

    /// <summary>The window menu, and with it the caption's close button.</summary>
    internal const long WS_SYSMENU = 0x00080000;

    /// <summary>lParam for WM_NCACTIVATE that changes the active state without repainting the frame.</summary>
    internal static readonly IntPtr NoFrameRepaint = new(-1);

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int valueSize);

    /// <summary>Sets an integer-valued DWM window attribute. Failures (older Windows) are ignored.</summary>
    internal static void SetDwmAttribute(IntPtr hwnd, int attribute, int value)
    {
        _ = DwmSetWindowAttribute(hwnd, attribute, ref value, sizeof(int));
    }

    /// <summary>Moves a window to a position in physical pixels without resizing, activating or restacking it.</summary>
    internal static void MoveWindowTo(IntPtr hWnd, double x, double y)
    {
        SetWindowPos(
            hWnd,
            IntPtr.Zero,
            (int)Math.Round(x),
            (int)Math.Round(y),
            0,
            0,
            SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

    /// <summary>Removes window styles (WS_*) from a window, keeping the rest.</summary>
    internal static void RemoveStyles(IntPtr hWnd, long styles)
    {
        if (IntPtr.Size == 8)
        {
            var current = GetWindowLongPtr64(hWnd, GWL_STYLE).ToInt64();
            SetWindowLongPtr64(hWnd, GWL_STYLE, new IntPtr(current & ~styles));
        }
        else
        {
            var current = GetWindowLong32(hWnd, GWL_STYLE);
            SetWindowLong32(hWnd, GWL_STYLE, current & ~(int)styles);
        }
    }

    /// <summary>Adds extended window styles (WS_EX_*) to a window, keeping the ones it has.</summary>
    internal static void AddExtendedStyles(IntPtr hWnd, long styles)
    {
        if (IntPtr.Size == 8)
        {
            var current = GetWindowLongPtr64(hWnd, GWL_EXSTYLE).ToInt64();
            SetWindowLongPtr64(hWnd, GWL_EXSTYLE, new IntPtr(current | styles));
        }
        else
        {
            var current = GetWindowLong32(hWnd, GWL_EXSTYLE);
            SetWindowLong32(hWnd, GWL_EXSTYLE, current | (int)styles);
        }
    }

    internal enum OwnerCheck
    {
        /// <summary>The window was already owned by the expected owner.</summary>
        Attached,

        /// <summary>It had come loose and was put back.</summary>
        Reattached,

        /// <summary>It could not be put back, or the window no longer exists.</summary>
        Failed,
    }

    /// <summary>
    /// Makes <paramref name="owner"/> the owner of <paramref name="hWnd"/> if it is not already,
    /// and reports whether that worked. <paramref name="previousOwner"/> is what it was owned by.
    /// </summary>
    internal static OwnerCheck EnsureOwner(IntPtr hWnd, IntPtr owner, out IntPtr previousOwner)
    {
        previousOwner = IntPtr.Zero;
        if (hWnd == IntPtr.Zero || !IsWindow(hWnd))
        {
            return OwnerCheck.Failed;
        }

        previousOwner = GetWindow(hWnd, GW_OWNER);
        if (previousOwner == owner)
        {
            return OwnerCheck.Attached;
        }

        // GWLP_HWNDPARENT on a top-level window sets its owner, despite the name.
        if (IntPtr.Size == 8)
        {
            SetWindowLongPtr64(hWnd, GWLP_HWNDPARENT, owner);
        }
        else
        {
            SetWindowLong32(hWnd, GWLP_HWNDPARENT, owner.ToInt32());
        }

        return GetWindow(hWnd, GW_OWNER) == owner ? OwnerCheck.Reattached : OwnerCheck.Failed;
    }

    /// <summary>The window class of <paramref name="hWnd"/>, for log messages.</summary>
    internal static string GetClassName(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero)
        {
            return "none";
        }

        var buffer = new char[128];
        var length = GetClassNameW(hWnd, buffer, buffer.Length);
        return length > 0 ? new string(buffer, 0, length) : "unknown";
    }

    private const uint GW_OWNER = 4;
    private const int GWLP_HWNDPARENT = -8;

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassNameW(IntPtr hWnd, [Out] char[] className, int maxCount);

    // The *Ptr variants only exist in 64-bit user32, hence the two pairs.

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
}
