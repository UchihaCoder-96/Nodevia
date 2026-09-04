using Nodevia.Models;
using Nodevia.Rendering;
using Nodevia.Routing;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Nodevia.Controls;

public class NodeCanvas : ItemsControl
{
    public static readonly Size NodeVisualSize = new(180, 100); // temporary, until Node has real size

    static NodeCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(typeof(NodeCanvas)));
    }

    public NodeCanvas()
    {
        Graph = new NodeGraph();
        Focusable = true;
    }

    // ============================================================
    // Template parts
    // ============================================================

    private Canvas? _transformRoot;
    private Rectangle? _selectionBox;
    private ScaleTransform? _scaleTransform;
    private TranslateTransform? _panTransform;
    private ConnectionLayer? _connectionLayer;
    private BackgroundLayer? _backgroundLayer;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _transformRoot = GetTemplateChild("PART_TransformRoot") as Canvas;
        _selectionBox = GetTemplateChild("PART_SelectionBox") as Rectangle;
        _scaleTransform = GetTemplateChild("PART_ScaleTransform") as ScaleTransform;
        _panTransform = GetTemplateChild("PART_PanTransform") as TranslateTransform;
        _connectionLayer = GetTemplateChild("PART_ConnectionLayer") as ConnectionLayer;
        _backgroundLayer = GetTemplateChild("PART_BackgroundLayer") as BackgroundLayer;

        if (_backgroundLayer is not null)
            _backgroundLayer.Renderer = BackgroundRenderer;

        if (_connectionLayer is not null)
        {
            _connectionLayer.PositionRoot = _transformRoot;
            _connectionLayer.PortControlLookup = FindPortControl;
            _connectionLayer.Route = Route;
            _connectionLayer.Graph = Graph;
        }

        SyncTransform();

        ItemContainerGenerator.StatusChanged -= OnItemContainerGeneratorStatusChanged;
        ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
    }

    private void OnItemContainerGeneratorStatusChanged(object? sender, EventArgs e)
    {
        if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            return;

        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)InvalidateConnections);
    }

    // ============================================================
    // Graph
    // ============================================================

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

        if (canvas._connectionLayer is not null)
            canvas._connectionLayer.Graph = canvas.Graph;

        if (e.OldValue is NodeGraph oldGraph)
            oldGraph.Connections.CollectionChanged -= canvas.OnConnectionsChanged;

        if (e.NewValue is NodeGraph newGraph)
            newGraph.Connections.CollectionChanged += canvas.OnConnectionsChanged;
    }

    private void OnConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _connectionLayer?.InvalidateVisual();
    }

    public void InvalidateConnections() => _connectionLayer?.InvalidateVisual();

    // ============================================================
    // Pan
    // ============================================================

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

    private void UpdatePan(MouseEventArgs e)
    {
        Point current = e.GetPosition(this);
        Vector delta = current - _panStartMouse;
        PanX = _panStartOffset.X + delta.X;
        PanY = _panStartOffset.Y + delta.Y;
    }

    private void EndPan()
    {
        if (!_isPanning)
            return;

        _isPanning = false;
        ReleaseMouseCapture();
    }

    // ============================================================
    // Zoom
    // ============================================================

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

    public Point ScreenToCanvas(Point screenPoint)
    {
        if (Zoom <= 0)
            return screenPoint;

        return new Point(
            (screenPoint.X - PanX) / Zoom,
            (screenPoint.Y - PanY) / Zoom);
    }

    public Point CanvasToScreen(Point canvasPoint)
    {
        return new Point(
            canvasPoint.X * Zoom + PanX,
            canvasPoint.Y * Zoom + PanY);
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

    // ============================================================
    // World size
    // ============================================================

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

    // ============================================================
    // Background rendering
    // ============================================================

    public static readonly DependencyProperty BackgroundRendererProperty =
        DependencyProperty.Register(nameof(BackgroundRenderer), typeof(CanvasBackgroundRenderer), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(new GridBackgroundRenderer(), OnBackgroundRendererChanged));

    public CanvasBackgroundRenderer BackgroundRenderer
    {
        get => (CanvasBackgroundRenderer)GetValue(BackgroundRendererProperty);
        set => SetValue(BackgroundRendererProperty, value);
    }

    private static void OnBackgroundRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (NodeCanvas)d;
        if (canvas._backgroundLayer is not null)
            canvas._backgroundLayer.Renderer = (CanvasBackgroundRenderer)e.NewValue;
    }

    // ============================================================
    // Selection / Z-order
    // (public API called by NodeControl)
    // ============================================================

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

            foreach (var c in Graph.Connections)
                c.IsSelected = false;

            InvalidateConnections();
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
    // Routing (pluggable connection curve)
    // ------------------------------------------------------------

    public static readonly DependencyProperty RouteProperty =
        DependencyProperty.Register(nameof(Route), typeof(ConnectionRoute), typeof(NodeCanvas),
            new FrameworkPropertyMetadata(new BezierConnectionRoute(), OnRouteChanged));

    public ConnectionRoute Route
    {
        get => (ConnectionRoute)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    private static void OnRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (NodeCanvas)d;
        if (canvas._connectionLayer is not null)
            canvas._connectionLayer.Route = (ConnectionRoute)e.NewValue;
    }

    // ------------------------------------------------------------
    // Port registry (Port -> PortControl lookup, kept in sync)
    // ------------------------------------------------------------

    private readonly Dictionary<Port, PortControl> _portControlsByPort = new();

    public void RegisterPortControl(PortControl control)
    {
        if (control.Port is not null)
            _portControlsByPort[control.Port] = control;
    }

    public void UnregisterPortControl(PortControl control)
    {
        if (control.Port is not null &&
            _portControlsByPort.TryGetValue(control.Port, out var existing) &&
            ReferenceEquals(existing, control))
        {
            _portControlsByPort.Remove(control.Port);
        }
    }

    private PortControl? FindPortControl(Port port) =>
        _portControlsByPort.TryGetValue(port, out var control) ? control : null;

    private PortControl? FindPortControlAt(Point pointInCanvasSpace)
    {
        var result = VisualTreeHelper.HitTest(this, pointInCanvasSpace);
        if (result?.VisualHit is not DependencyObject hit)
            return null;

        DependencyObject? current = hit;
        while (current is not null)
        {
            if (current is PortControl pc)
                return pc;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    // ------------------------------------------------------------
    // Drag-to-connect (called by PortControl)
    // ------------------------------------------------------------

    private bool _isConnectingPort;
    private PortControl? _connectionSourceControl;

    public void BeginConnectionDrag(PortControl sourceControl)
    {
        if (sourceControl.Port is null || _transformRoot is null)
            return;

        _isConnectingPort = true;
        _connectionSourceControl = sourceControl;

        Point sourcePos = sourceControl.GetCenterRelativeTo(_transformRoot);
        PortSide side = sourceControl.Port.Direction == PortDirection.Output ? PortSide.Right : PortSide.Left;

        _connectionLayer?.SetPreview(sourcePos, sourcePos, side);

        CaptureMouse();
    }

    private void UpdateConnectionPreview(MouseEventArgs e)
    {
        if (_transformRoot is null || _connectionSourceControl?.Port is not Port sourcePort)
            return;

        Point current = e.GetPosition(_transformRoot);
        Point sourcePos = _connectionSourceControl.GetCenterRelativeTo(_transformRoot);
        PortSide side = sourcePort.Direction == PortDirection.Output ? PortSide.Right : PortSide.Left;

        _connectionLayer?.SetPreview(sourcePos, current, side);
    }

    private void EndConnectionDrag(MouseButtonEventArgs e)
    {
        _isConnectingPort = false;
        _connectionLayer?.ClearPreview();
        ReleaseMouseCapture();

        Port? sourcePort = _connectionSourceControl?.Port;
        _connectionSourceControl = null;

        if (sourcePort is null)
            return;

        Point screenPos = e.GetPosition(this);
        PortControl? targetControl = FindPortControlAt(screenPos);

        if (targetControl?.Port is not Port targetPort || ReferenceEquals(targetPort, sourcePort))
            return;

        // Figure out input/output, regardless of which one the user grabbed first
        Port? output = null;
        Port? input = null;

        if (sourcePort.Direction == PortDirection.Output && targetPort.Direction == PortDirection.Input)
        {
            output = sourcePort;
            input = targetPort;
        }
        else if (sourcePort.Direction == PortDirection.Input && targetPort.Direction == PortDirection.Output)
        {
            output = targetPort;
            input = sourcePort;
        }

        if (output is null || input is null)
            return; // dropped on a same-direction port - not a valid connection

        try
        {
            Graph.Connect(output, input);
        }
        catch (InvalidOperationException)
        {
            // A visual "rejected" cue should go here, not essential for v1.
        }
    }

    private void CancelConnectionDrag()
    {
        if (!_isConnectingPort)
            return;

        _isConnectingPort = false;
        _connectionSourceControl = null;
        _connectionLayer?.ClearPreview();
    }

    // ============================================================
    // Mouse routing
    // ============================================================

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        switch (e.ChangedButton)
        {
            case MouseButton.Middle:
                BeginPan(e);
                break;

            case MouseButton.Left:
                if (!e.Handled)
                {
                    Connection? hitConnection = _transformRoot is not null
                        ? _connectionLayer?.HitTestConnection(e.GetPosition(_transformRoot))
                        : null;

                    if (hitConnection is not null)
                    {
                        HandleConnectionClick(hitConnection);
                        e.Handled = true;
                    }
                    else
                    {
                        BeginSelection(e);
                    }
                }
                break;
        }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);

        if (!IsKeyboardFocused)
            Focus();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_isConnectingPort)
        {
            UpdateConnectionPreview(e);
            return;
        }

        if (_isPanning)
        {
            UpdatePan(e);
            return;
        }

        if (_isSelecting)
            UpdateSelection(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.ChangedButton == MouseButton.Middle)
        {
            EndPan();
        }
        else if (e.ChangedButton == MouseButton.Left)
        {
            if (_isConnectingPort)
                EndConnectionDrag(e);
            else
                EndSelection();
        }
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        EndPan();
        EndSelection();
        CancelConnectionDrag();
    }

    // ============================================================
    // Keyboard shortcuts
    // ============================================================

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        switch (e.Key)
        {
            case Key.Delete:
                DeleteSelectedNodes();
                e.Handled = true;
                break;

            case Key.A when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                SelectAll();
                e.Handled = true;
                break;

            case Key.Escape:
                CancelActiveDrag();
                e.Handled = true;
                break;
        }
    }

    private void DeleteSelectedNodes()
    {
        var nodesToRemove = Graph.Nodes.Where(n => n.IsSelected).ToList();
        foreach (var node in nodesToRemove)
            Graph.Nodes.Remove(node);

        var connectionsToRemove = Graph.Connections.Where(c => c.IsSelected).ToList();
        foreach (var connection in connectionsToRemove)
            Graph.Disconnect(connection);
    }

    private void SelectAll()
    {
        foreach (var n in Graph.Nodes)
            n.IsSelected = true;
    }

    private void CancelActiveDrag()
    {
        if (_isConnectingPort)
        {
            CancelConnectionDrag();
            ReleaseMouseCapture();
            return;
        }

        if (_isSelecting)
        {
            EndSelection();
            return;
        }

        if (_isPanning)
        {
            EndPan();
        }
    }

    // ----------------------------------------
    // Connection Selection API
    // ----------------------------------------

    public IEnumerable<Connection> SelectedConnections => Graph.Connections.Where(c => c.IsSelected);

    private void HandleConnectionClick(Connection connection)
    {
        bool additive = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ||
                         Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

        if (additive)
        {
            connection.IsSelected = !connection.IsSelected;
        }
        else
        {
            foreach (var c in Graph.Connections)
                c.IsSelected = ReferenceEquals(c, connection);
        }

        InvalidateConnections();
    }

    public void ClearConnectionSelection()
    {
        bool anySelected = false;

        foreach (var c in Graph.Connections)
        {
            if (c.IsSelected)
                anySelected = true;

            c.IsSelected = false;
        }

        if (anySelected)
            InvalidateConnections();
    }
}

