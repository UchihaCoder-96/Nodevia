using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Nodevia.Controls;

public class NumericUpDown : System.Windows.Controls.Control
{
    static NumericUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    public static readonly DependencyProperty NumberProperty =
        DependencyProperty.Register(nameof(Number), typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public double Number
    {
        get => (double)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register(nameof(Min), typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.NegativeInfinity));

    public double Min
    {
        get => (double)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(nameof(Max), typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.PositiveInfinity));

    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(1.0));

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly DependencyProperty IsIntegerProperty =
        DependencyProperty.Register(nameof(IsInteger), typeof(bool), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(false));

    public bool IsInteger
    {
        get => (bool)GetValue(IsIntegerProperty);
        set => SetValue(IsIntegerProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_UpButton") is RepeatButton up)
            up.Click += (_, _) => Nudge(Step);

        if (GetTemplateChild("PART_DownButton") is RepeatButton down)
            down.Click += (_, _) => Nudge(-Step);
    }

    private void Nudge(double delta)
    {
        double result = Math.Clamp(Number + delta, Min, Max);
        Number = IsInteger ? Math.Round(result) : result;
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        double delta = e.Delta > 0 ? Step : -Step;
        Nudge(delta);

        e.Handled = true;
    }
}

