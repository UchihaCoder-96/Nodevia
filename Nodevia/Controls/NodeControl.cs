using Nodevia.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nodevia.Controls;

public class NodeControl : Control
{
    static NodeControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NodeControl),
            new FrameworkPropertyMetadata(typeof(NodeControl)));
    }

    public static readonly DependencyProperty NodeProperty =
        DependencyProperty.Register(
            nameof(Node),
            typeof(Node),
            typeof(NodeControl),
            new FrameworkPropertyMetadata(null));

    public Node? Node
    {
        get => (Node?)GetValue(NodeProperty);
        set => SetValue(NodeProperty, value);
    }

    private Canvas? _canvas;
    private Point _grabOffset;
    private bool _isDragging;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (Node is null)
            return;

        _canvas = FindAncestor<Canvas>(this);
        if (_canvas is null)
            return;

        // Where inside the node did we grab it (in Canvas space)
        Point mouseOnCanvas = e.GetPosition(_canvas);
        _grabOffset = new Point(
            mouseOnCanvas.X - Node.Position.X,
            mouseOnCanvas.Y - Node.Position.Y);

        _isDragging = true;
        CaptureMouse();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isDragging || _canvas is null || Node is null)
            return;

        Point mouseOnCanvas = e.GetPosition(_canvas);

        Node.Position = new Point(
            mouseOnCanvas.X - _grabOffset.X,
            mouseOnCanvas.Y - _grabOffset.Y);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        EndDrag();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        EndDrag();
    }

    private void EndDrag()
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        _canvas = null;
        ReleaseMouseCapture();
    }

    private static T? FindAncestor<T>(DependencyObject start) where T : DependencyObject
    {
        DependencyObject? current = VisualTreeHelper.GetParent(start);

        while (current is not null)
        {
            if (current is T match)
                return match;

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}

