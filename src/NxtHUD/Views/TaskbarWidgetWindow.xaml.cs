using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using NxtHUD.Core.Flyouts;
using NxtHUD.Core.ViewModels;
using NxtHUD.Interop;

namespace NxtHUD.Views;

/// <summary>
/// The compact now-playing widget that sits on the taskbar. TaskbarWidgetService creates,
/// positions and closes it; this class only holds the visuals.
/// </summary>
public partial class TaskbarWidgetWindow : Window
{
    /// <summary>Widest the title and artist get, in device-independent pixels.</summary>
    private const double FullTextWidth = 170;

    /// <summary>Title and artist width once space runs short.</summary>
    private const double NarrowTextWidth = 90;

    private TaskbarWidgetLayout _layout = TaskbarWidgetLayout.Full;

    public TaskbarWidgetWindow(TaskbarWidgetViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Returns the width, in device-independent pixels, the widget needs in each layout for the
    /// track it is showing now. Leaves the current layout as it was.
    /// </summary>
    public Func<TaskbarWidgetLayout, double> MeasureLayouts()
    {
        var widths = new Dictionary<TaskbarWidgetLayout, double>();
        var current = _layout;
        var available = new Size(double.PositiveInfinity, double.IsNaN(Height) ? double.PositiveInfinity : Height);
        foreach (var layout in Enum.GetValues<TaskbarWidgetLayout>())
        {
            SetParts(layout);
            LayoutRoot.Measure(available);
            widths[layout] = LayoutRoot.DesiredSize.Width;
        }

        SetParts(current);
        return layout => widths[layout];
    }

    /// <summary>Shows as much of the widget as <paramref name="layout"/> allows.</summary>
    public void ApplyLayout(TaskbarWidgetLayout layout)
    {
        if (layout != _layout)
        {
            _layout = layout;
            SetParts(layout);
        }
    }

    private void SetParts(TaskbarWidgetLayout layout)
    {
        TextPanel.MaxWidth = layout == TaskbarWidgetLayout.Full ? FullTextWidth : NarrowTextWidth;
        TextPanel.Visibility = layout == TaskbarWidgetLayout.Minimal ? Visibility.Collapsed : Visibility.Visible;
        CoverAndText.Margin = layout == TaskbarWidgetLayout.Minimal ? new Thickness(6, 0, 4, 0) : new Thickness(6, 0, 10, 0);
        VisualizerHost.Visibility = layout is TaskbarWidgetLayout.Full or TaskbarWidgetLayout.NarrowText
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>Uses light text on a dark taskbar and dark text on a light one.</summary>
    public void SetTaskbarTheme(bool isDark)
    {
        var brush = isDark ? Brushes.White : Brushes.Black;
        if (!ReferenceEquals(Foreground, brush))
        {
            Foreground = brush;
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // Never take focus, and stay out of Alt+Tab.
        var handle = new WindowInteropHelper(this).Handle;
        NativeMethods.AddExtendedStyles(handle, NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_TOOLWINDOW);
        TaskbarWindowRendering.UseSoftware(handle);
    }
}
