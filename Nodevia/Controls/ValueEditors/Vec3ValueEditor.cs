using Nodevia.Models;
using System.Windows;

namespace Nodevia.Controls.ValueEditors;

public class Vec3ValueEditor : PortValueEditor
{
    static Vec3ValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Vec3ValueEditor),
            new FrameworkPropertyMetadata(typeof(Vec3ValueEditor)));
    }

    public static readonly DependencyProperty XProperty =
        DependencyProperty.Register(nameof(X), typeof(double), typeof(Vec3ValueEditor),
            new FrameworkPropertyMetadata(0.0, OnComponentChanged));
    public double X { get => (double)GetValue(XProperty); set => SetValue(XProperty, value); }

    public static readonly DependencyProperty YProperty =
        DependencyProperty.Register(nameof(Y), typeof(double), typeof(Vec3ValueEditor),
            new FrameworkPropertyMetadata(0.0, OnComponentChanged));
    public double Y { get => (double)GetValue(YProperty); set => SetValue(YProperty, value); }

    public static readonly DependencyProperty ZProperty =
        DependencyProperty.Register(nameof(Z), typeof(double), typeof(Vec3ValueEditor),
            new FrameworkPropertyMetadata(0.0, OnComponentChanged));
    public double Z { get => (double)GetValue(ZProperty); set => SetValue(ZProperty, value); }

    private bool _isSyncingFromValue;

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        if (Value is not Vec3 vec)
            return;

        _isSyncingFromValue = true;
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        _isSyncingFromValue = false;
    }

    private static void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (Vec3ValueEditor)d;

        if (editor._isSyncingFromValue)
            return;

        editor.Value = new Vec3(editor.X, editor.Y, editor.Z);
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(Vec3ValueEditor),
            new FrameworkPropertyMetadata(false));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}

