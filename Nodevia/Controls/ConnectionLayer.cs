using Nodevia.Models;
using Nodevia.Routing;
using System.Windows;
using System.Windows.Media;

namespace Nodevia.Controls;

public class ConnectionLayer : FrameworkElement
{
    public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register(nameof(Graph), typeof(NodeGraph), typeof(ConnectionLayer),
            new FrameworkPropertyMetadata(null, OnInvalidatingChanged));

    public NodeGraph? Graph
    {
        get => (NodeGraph?)GetValue(GraphProperty);
        set => SetValue(GraphProperty, value);
    }

    public static readonly DependencyProperty RouteProperty =
        DependencyProperty.Register(nameof(Route), typeof(ConnectionRoute), typeof(ConnectionLayer),
            new FrameworkPropertyMetadata(null, OnInvalidatingChanged));

    public ConnectionRoute? Route
    {
        get => (ConnectionRoute?)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    public FrameworkElement? PositionRoot { get; set; }

    public Func<Port, PortControl?>? PortControlLookup { get; set; }

    private static readonly Pen ConnectionPen = new(Brushes.LightGray, 2.0);

    static ConnectionLayer()
    {
        ConnectionPen.Freeze();
    }

    private static void OnInvalidatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ConnectionLayer)d).InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        if (Graph is null || Route is null || PositionRoot is null || PortControlLookup is null)
            return;

        foreach (var connection in Graph.Connections)
        {
            PortControl? sourceControl = PortControlLookup(connection.Source);
            PortControl? targetControl = PortControlLookup(connection.Target);

            if (sourceControl is null || targetControl is null)
                continue;

            Point start = sourceControl.GetCenterRelativeTo(PositionRoot);
            Point end = targetControl.GetCenterRelativeTo(PositionRoot);

            Geometry geometry = Route.BuildGeometry(start, end, PortSide.Right, PortSide.Left);
            dc.DrawGeometry(null, ConnectionPen, geometry);
        }

        if (_previewStart is Point _start && _previewEnd is Point _end && Route is not null)
        {
            Geometry previewGeometry = Route.BuildGeometry(_start, _end, _previewStartSide, Opposite(_previewStartSide));
            dc.DrawGeometry(null, PreviewPen, previewGeometry);
        }
    }

    private Point? _previewStart;
    private Point? _previewEnd;
    private PortSide _previewStartSide;

    private static readonly Pen PreviewPen = CreatePreviewPen();

    private static Pen CreatePreviewPen()
    {
        var pen = new Pen(Brushes.White, 2.0) { DashStyle = DashStyles.Dash };
        pen.Freeze();
        return pen;
    }

    public void SetPreview(Point start, Point end, PortSide startSide)
    {
        _previewStart = start;
        _previewEnd = end;
        _previewStartSide = startSide;
        InvalidateVisual();
    }

    public void ClearPreview()
    {
        if (_previewStart is null)
            return;

        _previewStart = null;
        _previewEnd = null;
        InvalidateVisual();
    }

    private static PortSide Opposite(PortSide side) => side switch
    {
        PortSide.Left => PortSide.Right,
        PortSide.Right => PortSide.Left,
        PortSide.Top => PortSide.Bottom,
        PortSide.Bottom => PortSide.Top,
        _ => side
    };
}

