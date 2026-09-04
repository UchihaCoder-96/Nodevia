using System.Windows;
using System.Windows.Media;

namespace Nodevia.Rendering;

public abstract class CanvasBackgroundRenderer
{
    public abstract void Render(DrawingContext dc, Rect viewport, double panX, double panY, double zoom);
}

