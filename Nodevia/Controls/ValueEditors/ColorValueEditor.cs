using Nodevia.Models;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Nodevia.Controls.ValueEditors;

public class ColorValueEditor : PortValueEditor
{
    static ColorValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(typeof(ColorValueEditor)));
    }

    public static readonly DependencyProperty RProperty =
        DependencyProperty.Register(nameof(R), typeof(double), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(1.0, OnComponentChanged));
    public double R { get => (double)GetValue(RProperty); set => SetValue(RProperty, value); }

    public static readonly DependencyProperty GProperty =
        DependencyProperty.Register(nameof(G), typeof(double), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(1.0, OnComponentChanged));
    public double G { get => (double)GetValue(GProperty); set => SetValue(GProperty, value); }

    public static readonly DependencyProperty BProperty =
        DependencyProperty.Register(nameof(B), typeof(double), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(1.0, OnComponentChanged));
    public double B { get => (double)GetValue(BProperty); set => SetValue(BProperty, value); }

    public static readonly DependencyProperty AProperty =
        DependencyProperty.Register(nameof(A), typeof(double), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(1.0, OnComponentChanged));
    public double A { get => (double)GetValue(AProperty); set => SetValue(AProperty, value); }

    public static readonly DependencyProperty HexProperty =
        DependencyProperty.Register(nameof(Hex), typeof(string), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata("#FFFFFFFF", OnHexChanged));
    public string Hex { get => (string)GetValue(HexProperty); set => SetValue(HexProperty, value); }

    public static readonly DependencyProperty IsPickerOpenProperty =
        DependencyProperty.Register(nameof(IsPickerOpen), typeof(bool), typeof(ColorValueEditor),
            new FrameworkPropertyMetadata(false));
    public bool IsPickerOpen { get => (bool)GetValue(IsPickerOpenProperty); set => SetValue(IsPickerOpenProperty, value); }

    private bool _isSyncingFromValue;

    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        if (Value is not Color color)
            return;

        _isSyncingFromValue = true;
        R = color.ScR;
        G = color.ScG;
        B = color.ScB;
        A = color.ScA;
        Hex = color.ToString();
        _isSyncingFromValue = false;
    }

    private static void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (ColorValueEditor)d;

        if (editor._isSyncingFromValue)
            return;

        editor.Value = Color.FromScRgb((float)editor.A, (float)editor.R, (float)editor.G, (float)editor.B);
    }

    private static void OnHexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (ColorValueEditor)d;

        if (editor._isSyncingFromValue)
            return;

        try
        {
            if (ColorConverter.ConvertFromString((string)e.NewValue) is Color color)
                editor.Value = color;
        }
        catch
        {
            // Invalid hex text - leave the current Value untouched for v1.
        }
    }
}


