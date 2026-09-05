using Nodevia.Models;
using System.Windows;
using System.Windows.Input;

namespace Nodevia.Controls.ValueEditors;

public class NumericUpDownValueEditor : PortValueEditor
{
    static NumericUpDownValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(typeof(NumericUpDownValueEditor)));
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register(nameof(Min), typeof(double), typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(double.NegativeInfinity));

    public double Min
    {
        get => (double)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(nameof(Max), typeof(double), typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(double.PositiveInfinity));

    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(double), typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(1.0));

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly DependencyProperty IsIntegerProperty =
        DependencyProperty.Register(nameof(IsInteger), typeof(bool), typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(false));

    public bool IsInteger
    {
        get => (bool)GetValue(IsIntegerProperty);
        set => SetValue(IsIntegerProperty, value);
    }

    public static readonly DependencyProperty NumberProperty =
        DependencyProperty.Register(nameof(Number), typeof(double), typeof(NumericUpDownValueEditor),
            new FrameworkPropertyMetadata(0.0, OnNumberChanged));

    public double Number
    {
        get => (double)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    private bool _isSyncingFromValue;

    protected override void OnPortAttached(Port? port)
    {
        base.OnPortAttached(port);

        if (port is null)
            return;

        if (port.Metadata.TryGetValue("Numeric.Min", out var min) && min is double minVal)
            Min = minVal;

        if (port.Metadata.TryGetValue("Numeric.Max", out var max) && max is double maxVal)
            Max = maxVal;

        if (port.Metadata.TryGetValue("Numeric.Step", out var step) && step is double stepVal)
            Step = stepVal;

        if (port.Metadata.TryGetValue("Numeric.IsInteger", out var isInt) && isInt is bool isIntVal)
            IsInteger = isIntVal;
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        if (Value is null)
            return;

        double asDouble = System.Convert.ToDouble(Value);

        _isSyncingFromValue = true;
        Number = asDouble;
        _isSyncingFromValue = false;
    }

    private static void OnNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (NumericUpDownValueEditor)d;

        if (editor._isSyncingFromValue)
            return;

        double clamped = Math.Clamp((double)e.NewValue, editor.Min, editor.Max);

        editor.Value = editor.IsInteger ? (object)(int)Math.Round(clamped) : clamped;

        if (Math.Abs(clamped - (double)e.NewValue) > double.Epsilon)
        {
            editor._isSyncingFromValue = true;
            editor.Number = clamped;
            editor._isSyncingFromValue = false;
        }
    }

    public void Increment() => Number = Number + Step;
    public void Decrement() => Number = Number - Step;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_UpButton") is System.Windows.Controls.Primitives.RepeatButton up)
            up.Click += (_, _) => Increment();

        if (GetTemplateChild("PART_DownButton") is System.Windows.Controls.Primitives.RepeatButton down)
            down.Click += (_, _) => Decrement();
    }
}

