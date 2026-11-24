using System.Windows;
using System.Windows.Interop;

namespace NxtHUD.Views;

/// <summary>Set-up shared by the see-through taskbar windows (the widget and the stats panel).</summary>
internal static class TaskbarWindowRendering
{
    /// <summary>
    /// Makes sure <paramref name="window"/> does not become the app's main window.
    /// </summary>
    /// <remarks>
    /// WPF makes the first window created the main window, and the stats panel is usually the
    /// first. On every Windows colour or theme change (energy saver switching transparency
    /// off, the settings Windows applies after sign-in) WPF-UI's theme refresh re-themes the
    /// main window's background, which turned the panel into a gray box: white at about a third
    /// opacity. The flyouts, the tray menu owner and the settings window already step aside the
    /// same way.
    /// </remarks>
    public static void NeverMainWindow(Window window)
    {
        if (Application.Current is { } app && ReferenceEquals(app.MainWindow, window))
        {
            app.MainWindow = null;
        }
    }

    /// <summary>
    /// Draws the window with the processor instead of the graphics card.
    /// </summary>
    /// <remarks>
    /// A see-through (layered) WPF window drawn by the graphics card keeps its picture in a
    /// graphics-card surface. When the graphics are reset, as on waking from sleep, during
    /// sign-in while the driver settles, or when a laptop switches graphics chips, Windows can
    /// drop that surface and the window's clear parts turn solid gray while its text keeps
    /// updating. Drawn by the processor the window has no such surface to lose. Both windows
    /// are small, so the extra processor work is negligible.
    /// </remarks>
    public static void UseSoftware(IntPtr handle)
    {
        if (HwndSource.FromHwnd(handle)?.CompositionTarget is { } target)
        {
            target.RenderMode = RenderMode.SoftwareOnly;
        }
    }
}
