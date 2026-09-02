using Nodevia.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nodevia.Controls;

public class NodeCanvas : ItemsControl
{
    static NodeCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(typeof(NodeCanvas)));
    }

    public ObservableCollection<Node> Nodes { get; } = new();

    public NodeCanvas()
    {
        ItemsSource = Nodes;
    }

    private TranslateTransform? _panTransform;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _panTransform = GetTemplateChild("PART_PanTransform") as TranslateTransform;
        SyncPanTransform();
    }

    public static readonly DependencyProperty PanXProperty =
        DependencyProperty.Register(
            nameof(PanX),
            typeof(double),
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(0.0, OnPanChanged));

    public double PanX
    {
        get => (double)GetValue(PanXProperty);
        set => SetValue(PanXProperty, value);
    }

    public static readonly DependencyProperty PanYProperty =
        DependencyProperty.Register(
            nameof(PanY),
            typeof(double),
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(0.0, OnPanChanged));

    public double PanY
    {
        get => (double)GetValue(PanYProperty);
        set => SetValue(PanYProperty, value);
    }

    private static void OnPanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NodeCanvas)d).SyncPanTransform();
    }

    private void SyncPanTransform()
    {
        if (_panTransform is null)
            return;

        _panTransform.X = PanX;
        _panTransform.Y = PanY;
    }

    public static readonly DependencyProperty WorldWidthProperty =
        DependencyProperty.Register(
            nameof(WorldWidth),
            typeof(double),
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(5000.0));

    public double WorldWidth
    {
        get => (double)GetValue(WorldWidthProperty);
        set => SetValue(WorldWidthProperty, value);
    }

    public static readonly DependencyProperty WorldHeightProperty =
        DependencyProperty.Register(
            nameof(WorldHeight),
            typeof(double),
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(5000.0));

    public double WorldHeight
    {
        get => (double)GetValue(WorldHeightProperty);
        set => SetValue(WorldHeightProperty, value);
    }

    private bool _isPanning;
    private Point _panStartMouse;
    private Point _panStartOffset;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton != MouseButton.Middle)
            return;

        _isPanning = true;
        _panStartMouse = e.GetPosition(this);
        _panStartOffset = new Point(PanX, PanY);

        CaptureMouse();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isPanning)
            return;

        Point current = e.GetPosition(this);
        Vector delta = current - _panStartMouse;

        PanX = _panStartOffset.X + delta.X;
        PanY = _panStartOffset.Y + delta.Y;
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.ChangedButton != MouseButton.Middle)
            return;

        EndPan();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        EndPan();
    }

    private void EndPan()
    {
        if (!_isPanning)
            return;

        _isPanning = false;
        ReleaseMouseCapture();
    }
}

