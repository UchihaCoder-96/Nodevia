using System.Windows;
using System.Windows.Media;

namespace Nodevia.Routing;

public abstract class ConnectionRoute
{
    public abstract Geometry BuildGeometry(Point start, Point end, PortSide startSide, PortSide endSide);
}