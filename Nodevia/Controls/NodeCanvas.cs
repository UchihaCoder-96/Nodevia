using Nodevia.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nodevia.Controls;

public class NodeCanvas : ItemsControl
{
    public static readonly Size NodeVisualSize = new(180, 100); // temporary

    static NodeCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(typeof(NodeCanvas)));
    }

    public NodeCanvas()
    {
        Graph = new NodeGraph();
    }

    private Canvas? _transformRoot;
    private Rectangle? _selectionBox;
    private ScaleTransform? _scaleTransform;
    private TranslateTransform? _panTransform;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _transformRoot = GetTemplateChild("PART_TransformRoot") as Canvas;
        _selectionBox = GetTemplateChild("PART_SelectionBox") as Rectangle;
        _scaleTransform = GetTemplateChild("PART_ScaleTransform") as ScaleTransform;
        _panTransform = GetTemplateChild("PART_PanTransform") as TranslateTransform;

        SyncTransform();
    }

    // ------------------------------------------------------------
    // Graph
    // ------------------------------------------------------------

    public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register(
            nameof(Graph),
            typeof(NodeGraph),
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(null, OnGraphChanged));

    public NodeGraph Graph
    {
        get => (NodeGraph)GetValue(GraphProperty);
        set => SetValue(GraphProperty, value);
    }

    private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (NodeCanvas)d;
        canvas.ItemsSource = canvas.Graph?.Nodes;
    }

    // ------------------------------------------------------------
    // Pan
    // ------------------------------------------------------------

    public static readonly DependencyProperty PanXProperty =
        DependencyProperty.Register(nameof(PanX), typeof(double), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(0.0, OnTransformChanged));

    public double PanX
    {
        get => (double)GetValue(PanXProperty);
        set => SetValue(PanXProperty, value);
    }

    public static readonly DependencyProperty PanYProperty =
        DependencyProperty.Register(nameof(PanY), typeof(double), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(0.0, OnTransformChanged));

    public double PanY
    {
        get => (double)GetValue(PanYProperty);
        set => SetValue(PanYProperty, value);
    }

    // ------------------------------------------------------------
    // Zoom
    // ------------------------------------------------------------

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(1.0, OnTransformChanged));

    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public double MinZoom { get; set; } = 0.1;
    public double MaxZoom { get; set; } = 3.0;

    private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NodeCanvas)d).SyncTransform();
    }

    private void SyncTransform()
    {
        if (_scaleTransform is not null)
        {
            _scaleTransform.ScaleX = Zoom;
            _scaleTransform.ScaleY = Zoom;
        }

        if (_panTransform is not null)
        {
            _panTransform.X = PanX;
            _panTransform.Y = PanY;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        double oldZoom = Zoom;
        double factor = e.Delta > 0 ? 1.1 : 1.0 / 1.1;
        double newZoom = Math.Clamp(oldZoom * factor, MinZoom, MaxZoom);

        if (Math.Abs(newZoom - oldZoom) < 0.0001)
            return;

        // Screen-space point (relative to this control, unaffected by the transform).
        Point screenPos = e.GetPosition(this);

        // World point currently under the cursor.
        Point worldPoint = new(
            (screenPos.X - PanX) / oldZoom,
            (screenPos.Y - PanY) / oldZoom);

        // Re-solve pan so that same world point stays under the cursor at the new zoom.
        PanX = screenPos.X - worldPoint.X * newZoom;
        PanY = screenPos.Y - worldPoint.Y * newZoom;
        Zoom = newZoom;

        e.Handled = true;
    }

    // ------------------------------------------------------------
    // World size
    // ------------------------------------------------------------

    public static readonly DependencyProperty WorldWidthProperty =
        DependencyProperty.Register(nameof(WorldWidth), typeof(double), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(5000.0));

    public double WorldWidth
    {
        get => (double)GetValue(WorldWidthProperty);
        set => SetValue(WorldWidthProperty, value);
    }

    public static readonly DependencyProperty WorldHeightProperty =
        DependencyProperty.Register(nameof(WorldHeight), typeof(double), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(5000.0));

    public double WorldHeight
    {
        get => (double)GetValue(WorldHeightProperty);
        set => SetValue(WorldHeightProperty, value);
    }

    // ------------------------------------------------------------
    // Selection / Z-order
    // ------------------------------------------------------------

    private int _nextZIndex = 1;

    public IEnumerable<Node> SelectedNodes => Graph.Nodes.Where(n => n.IsSelected);

    public void BringToFront(Node node) => node.ZIndex = _nextZIndex++;

    public void SelectOnly(Node node)
    {
        foreach (var n in Graph.Nodes)
            n.IsSelected = ReferenceEquals(n, node);
    }

    public void ToggleSelection(Node node) => node.IsSelected = !node.IsSelected;

    // ------------------------------------------------------------
    // Panning (middle mouse)
    // ------------------------------------------------------------

    private bool _isPanning;
    private Point _panStartMouse;
    private Point _panStartOffset;

    private void BeginPan(MouseButtonEventArgs e)
    {
        _isPanning = true;
        _panStartMouse = e.GetPosition(this);
        _panStartOffset = new Point(PanX, PanY);
        CaptureMouse();
        e.Handled = true;
    }

    private void EndPan()
    {
        if (!_isPanning)
            return;

        _isPanning = false;
        ReleaseMouseCapture();
    }

    // ------------------------------------------------------------
    // Rubber-band selection (left mouse on empty space)
    // ------------------------------------------------------------

    private bool _isSelecting;
    private Point _selectionStartWorld;
    private bool _additiveSelection;

    private void BeginSelection(MouseButtonEventArgs e)
    {
        if (_transformRoot is null)
            return;

        _isSelecting = true;
        _additiveSelection = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ||
                              Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

        _selectionStartWorld = e.GetPosition(_transformRoot);

        if (_selectionBox is not null)
        {
            Canvas.SetLeft(_selectionBox, _selectionStartWorld.X);
            Canvas.SetTop(_selectionBox, _selectionStartWorld.Y);
            _selectionBox.Width = 0;
            _selectionBox.Height = 0;
            _selectionBox.Visibility = Visibility.Visible;
        }

        if (!_additiveSelection)
        {
            foreach (var n in Graph.Nodes)
                n.IsSelected = false;
        }

        CaptureMouse();
        e.Handled = true;
    }

    private void UpdateSelection(MouseEventArgs e)
    {
        if (_transformRoot is null || _selectionBox is null)
            return;

        Point current = e.GetPosition(_transformRoot);

        double x = Math.Min(current.X, _selectionStartWorld.X);
        double y = Math.Min(current.Y, _selectionStartWorld.Y);
        double w = Math.Abs(current.X - _selectionStartWorld.X);
        double h = Math.Abs(current.Y - _selectionStartWorld.Y);

        Canvas.SetLeft(_selectionBox, x);
        Canvas.SetTop(_selectionBox, y);
        _selectionBox.Width = w;
        _selectionBox.Height = h;

        var rect = new Rect(x, y, w, h);

        foreach (var n in Graph.Nodes)
        {
            bool intersects = new Rect(n.Position, NodeVisualSize).IntersectsWith(rect);

            if (intersects)
                n.IsSelected = true;
            else if (!_additiveSelection)
                n.IsSelected = false;
        }
    }

    private void EndSelection()
    {
        if (!_isSelecting)
            return;

        _isSelecting = false;

        if (_selectionBox is not null)
            _selectionBox.Visibility = Visibility.Collapsed;

        ReleaseMouseCapture();
    }

    // ------------------------------------------------------------
    // Mouse routing
    // ------------------------------------------------------------

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        switch (e.ChangedButton)
        {
            case MouseButton.Middle:
                BeginPan(e);
                break;

            case MouseButton.Left:
                if (!e.Handled) // a node already handled its own click
                    BeginSelection(e);
                break;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_isPanning)
        {
            Point current = e.GetPosition(this);
            Vector delta = current - _panStartMouse;
            PanX = _panStartOffset.X + delta.X;
            PanY = _panStartOffset.Y + delta.Y;
            return;
        }

        if (_isSelecting)
            UpdateSelection(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.ChangedButton == MouseButton.Middle)
            EndPan();
        else if (e.ChangedButton == MouseButton.Left)
            EndSelection();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        EndPan();
        EndSelection();
    }
}

