using Nodevia.Models;
using System.Windows;

namespace Nodevia.Controls.ValueEditors;

public class Vec2ValueEditor : PortValueEditor
{
    static Vec2ValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Vec2ValueEditor),
            new FrameworkPropertyMetadata(typeof(Vec2ValueEditor)));
    }

    public static readonly DependencyProperty XProperty =
        DependencyProperty.Register(nameof(X), typeof(double), typeof(Vec2ValueEditor),
            new FrameworkPropertyMetadata(0.0, OnComponentChanged));

    public double X
    {
        get => (double)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public static readonly DependencyProperty YProperty =
        DependencyProperty.Register(nameof(Y), typeof(double), typeof(Vec2ValueEditor),
            new FrameworkPropertyMetadata(0.0, OnComponentChanged));

    public double Y
    {
        get => (double)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    private bool _isSyncingFromValue;

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        if (Value is not Vec2 vec)
            return;

        _isSyncingFromValue = true;
        X = vec.X;
        Y = vec.Y;
        _isSyncingFromValue = false;
    }

    private static void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (Vec2ValueEditor)d;

        if (editor._isSyncingFromValue)
            return;

        editor.Value = new Vec2(editor.X, editor.Y);
    }
}

