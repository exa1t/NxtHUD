using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using NxtHUD.Core.Flyouts;
using NxtHUD.Core.ViewModels;
using NxtHUD.Interop;

namespace NxtHUD.Views;

/// <summary>
/// The system stats panel that sits on the taskbar next to the tray. TaskbarStatsService
/// creates, positions and closes it; this class shows the view model's two lines in the layout
/// the service picks.
/// </summary>
public partial class TaskbarStatsWindow : Window
{
    private readonly TaskbarStatsViewModel _viewModel;
    private TaskbarStatsLayout _layout = TaskbarStatsLayout.TwoLines;

    public TaskbarStatsWindow(TaskbarStatsViewModel viewModel)
    {
        InitializeComponent();
        TaskbarWindowRendering.NeverMainWindow(this);
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Closed += (_, _) => _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        ShowLines();
    }

    /// <summary>Raised when the text changed, which can change how wide the panel needs to be.</summary>
    public event EventHandler? TextChanged;

    /// <summary>Uses light text on a dark taskbar and dark text on a light one.</summary>
    public void SetTaskbarTheme(bool isDark)
    {
        var brush = isDark ? Brushes.White : Brushes.Black;
        if (!ReferenceEquals(Foreground, brush))
        {
            Foreground = brush;
        }
    }

    /// <summary>The width, in device-independent pixels, each layout needs for the current text.</summary>
    public Func<TaskbarStatsLayout, double> MeasureLayouts()
    {
        var widths = new Dictionary<TaskbarStatsLayout, double>();
        var current = _layout;
        var available = new Size(double.PositiveInfinity, double.IsNaN(Height) ? double.PositiveInfinity : Height);
        foreach (var layout in Enum.GetValues<TaskbarStatsLayout>())
        {
            _layout = layout;
            ShowLines();
            LayoutRoot.Measure(available);
            widths[layout] = LayoutRoot.DesiredSize.Width;
        }

        _layout = current;
        ShowLines();
        return layout => widths[layout];
    }

    public void ApplyLayout(TaskbarStatsLayout layout)
    {
        if (layout != _layout)
        {
            _layout = layout;
            ShowLines();
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

    /// <summary>
    /// Two lines when both have content and there is room; otherwise only the first line that
    /// has anything in it.
    /// </summary>
    private void ShowLines()
    {
        var first = _viewModel.Line1;
        var second = _viewModel.Line2;

        if (_layout == TaskbarStatsLayout.OneLine || string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
        {
            SetText(FirstLine, string.IsNullOrEmpty(first) ? second : first);
            SetText(SecondLine, string.Empty);
        }
        else
        {
            SetText(FirstLine, first);
            SetText(SecondLine, second);
        }

        SecondLine.Visibility = string.IsNullOrEmpty(SecondLine.Text) ? Visibility.Collapsed : Visibility.Visible;
    }

    private static void SetText(System.Windows.Controls.TextBlock block, string text)
    {
        if (block.Text != text)
        {
            block.Text = text;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TaskbarStatsViewModel.Line1) or nameof(TaskbarStatsViewModel.Line2))
        {
            ShowLines();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnClicked(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.OpenDetailsCommand.CanExecute(null))
        {
            _viewModel.OpenDetailsCommand.Execute(null);
        }
    }
}
