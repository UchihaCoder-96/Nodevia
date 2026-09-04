using System.Windows;
using System.Windows.Media;

namespace Nodevia.Routing;

public class BezierConnectionRoute : ConnectionRoute
{
    public double Tension { get; set; } = 0.5;
    public double MinOffset { get; set; } = 40;

    public override Geometry BuildGeometry(Point start, Point end, PortSide startSide, PortSide endSide)
    {
        Vector startDir = DirectionFor(startSide);
        Vector endDir = DirectionFor(endSide);

        double distance = Math.Max(
            Math.Abs(end.X - start.X),
            Math.Abs(end.Y - start.Y));

        double offset = Math.Max(distance * Tension, MinOffset);

        Point control1 = start + startDir * offset;
        Point control2 = end + endDir * offset;

        var figure = new PathFigure { StartPoint = start, IsClosed = false };
        figure.Segments.Add(new BezierSegment(control1, control2, end, isStroked: true));

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }

    private static Vector DirectionFor(PortSide side) => side switch
    {
        PortSide.Left => new Vector(-1, 0),
        PortSide.Right => new Vector(1, 0),
        PortSide.Top => new Vector(0, -1),
        PortSide.Bottom => new Vector(0, 1),
        _ => new Vector(1, 0)
    };
}


