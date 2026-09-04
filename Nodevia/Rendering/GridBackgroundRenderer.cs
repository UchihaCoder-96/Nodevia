using System.Windows;
using System.Windows.Media;

namespace Nodevia.Rendering;

public class GridBackgroundRenderer : CanvasBackgroundRenderer
{
    public double CellSize { get; set; } = 40;
    public Brush LineBrush { get; set; } = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255));
    public double LineThickness { get; set; } = 1.0;

    public override void Render(DrawingContext dc, Rect viewport, double panX, double panY, double zoom)
    {
        double cell = CellSize * zoom;
        if (cell < 4)
            return; // too dense to be usefulat extreme zoom-out - skip drawing

        var pen = new Pen(LineBrush, LineThickness);
        pen.Freeze();

        double offsetX = panX % cell;
        double offsetY = panY % cell;

        for (double x = offsetX; x < viewport.Width; x += cell)
            dc.DrawLine(pen, new Point(x, 0), new Point(x, viewport.Height));

        for (double y = offsetY; y < viewport.Height; y += cell)
            dc.DrawLine(pen, new Point(0, y), new Point(viewport.Width, y));
    }
}


