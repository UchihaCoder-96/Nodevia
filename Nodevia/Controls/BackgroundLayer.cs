using Nodevia.Rendering;
using System.Windows;
using System.Windows.Media;

namespace Nodevia.Controls;

public class BackgroundLayer : FrameworkElement
{
    public static readonly DependencyProperty RendererProperty =
        DependencyProperty.Register(nameof(Renderer), typeof(CanvasBackgroundRenderer), typeof(BackgroundLayer),
            new FrameworkPropertyMetadata(null, OnInvalidatingChanged));

    public CanvasBackgroundRenderer? Renderer
    {
        get => (CanvasBackgroundRenderer?)GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    public static readonly DependencyProperty PanXProperty =
        DependencyProperty.Register(nameof(PanX), typeof(double), typeof(BackgroundLayer),
            new FrameworkPropertyMetadata(0.0, OnInvalidatingChanged));

    public double PanX
    {
        get => (double)GetValue(PanXProperty);
        set => SetValue(PanXProperty, value);
    }

    public static readonly DependencyProperty PanYProperty =
        DependencyProperty.Register(nameof(PanY), typeof(double), typeof(BackgroundLayer),
            new FrameworkPropertyMetadata(0.0, OnInvalidatingChanged));

    public double PanY
    {
        get => (double)GetValue(PanYProperty);
        set => SetValue(PanYProperty, value);
    }

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(BackgroundLayer),
            new FrameworkPropertyMetadata(1.0, OnInvalidatingChanged));

    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    private static void OnInvalidatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BackgroundLayer)d).InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        if (Renderer is null)
            return;

        var viewport = new Rect(0, 0, ActualWidth, ActualHeight);
        Renderer.Render(dc, viewport, PanX, PanY, Zoom);
    }
}

