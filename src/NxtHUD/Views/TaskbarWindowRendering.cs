using System.Windows.Interop;

namespace NxtHUD.Views;

/// <summary>How the see-through taskbar windows (the widget and the stats panel) are drawn.</summary>
internal static class TaskbarWindowRendering
{
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
