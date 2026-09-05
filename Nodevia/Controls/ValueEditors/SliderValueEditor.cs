using Nodevia.Models;
using System.Windows;

namespace Nodevia.Controls.ValueEditors;

public class SliderValueEditor : PortValueEditor
{
    static SliderValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SliderValueEditor),
            new FrameworkPropertyMetadata(typeof(SliderValueEditor)));
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register(nameof(Min), typeof(double), typeof(SliderValueEditor),
            new FrameworkPropertyMetadata(0.0));

    public double Min
    {
        get => (double)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(nameof(Max), typeof(double), typeof(SliderValueEditor),
            new FrameworkPropertyMetadata(1.0));

    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(double), typeof(SliderValueEditor),
            new FrameworkPropertyMetadata(0.01));

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    protected override void OnPortAttached(Port? port)
    {
        base.OnPortAttached(port);

        if (port is null)
            return;

        if (port.Metadata.TryGetValue("Slider.Min", out var min) && min is double minVal)
            Min = minVal;

        if (port.Metadata.TryGetValue("Slider.Max", out var max) && max is double maxVal)
            Max = maxVal;

        if (port.Metadata.TryGetValue("Slider.Step", out var step) && step is double stepVal)
            Step = stepVal;
    }
}

