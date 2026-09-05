using Nodevia.Commands;
using Nodevia.Models;
using System.ComponentModel;
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
    DependencyProperty.Register(nameof(Node), typeof(Node), typeof(NodeControl),
        new FrameworkPropertyMetadata(null, OnNodeChanged));

    public Node? Node
    {
        get => (Node?)GetValue(NodeProperty);
        set => SetValue(NodeProperty, value);
    }

    public static readonly DependencyProperty IsNodeSelectedProperty =
    DependencyProperty.Register(nameof(IsNodeSelected), typeof(bool), typeof(NodeControl),
        new FrameworkPropertyMetadata(false));

    public bool IsNodeSelected
    {
        get => (bool)GetValue(IsNodeSelectedProperty);
        private set => SetValue(IsNodeSelectedProperty, value);
    }

    private static void OnNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (NodeControl)d;

        if (e.OldValue is Node oldNode)
            oldNode.PropertyChanged -= control.OnNodePropertyChanged;

        if (e.NewValue is Node newNode)
        {
            newNode.PropertyChanged += control.OnNodePropertyChanged;
            control.IsNodeSelected = newNode.IsSelected;
        }
        else
        {
            control.IsNodeSelected = false;
        }
    }

    private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Node.IsSelected) && Node is not null)
            IsNodeSelected = Node.IsSelected;
    }

    private Canvas? _canvas;
    private NodeCanvas? _ownerCanvas;
    private bool _isDragging;
    private Point _dragStartMouseCanvas;
    private readonly Dictionary<Node, Point> _dragStartPositions = new();

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.Handled) // a child PortControl already started a connection drag
            return;

        if (e.OriginalSource is DependencyObject source && IsInsideValueEditor(source))
        {
            e.Handled = true; // let the editor (including an open ComboBox dropdown) handle its own click
            return;
        }

        if (Node is null)
            return;

        _canvas = FindAncestor<Canvas>(this);
        _ownerCanvas = FindAncestor<NodeCanvas>(this);
        if (_canvas is null || _ownerCanvas is null)
            return;

        _ownerCanvas.BringToFront(Node);

        bool ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        if (ctrl)
        {
            _ownerCanvas.ToggleSelection(Node);
        }
        else
        {
            _ownerCanvas.ClearConnectionSelection();

            if (!Node.IsSelected)
                _ownerCanvas.SelectOnly(Node);
            // else: already selected, no ctrl -> keep selection so the whole group can drag
        }

        _dragStartMouseCanvas = e.GetPosition(_canvas);
        _dragStartPositions.Clear();

        IEnumerable<Node> toDrag = Node.IsSelected ? _ownerCanvas.SelectedNodes : new[] { Node };
        foreach (var n in toDrag)
            _dragStartPositions[n] = n.Position;

        _isDragging = true;
        CaptureMouse();
        e.Handled = true;
    }

    private static bool IsInsideValueEditor(DependencyObject start)
    {
        DependencyObject? current = start;

        while (current is not null)
        {
            if (current is TextBox or CheckBox or ComboBox)
                return true;

            current = LogicalTreeHelper.GetParent(current) ?? VisualTreeHelper.GetParent(current);
        }

        return false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isDragging || _canvas is null)
            return;

        Point current = e.GetPosition(_canvas);
        Vector delta = current - _dragStartMouseCanvas;

        foreach (var kvp in _dragStartPositions)
            kvp.Key.Position = new Point(kvp.Value.X + delta.X, kvp.Value.Y + delta.Y);

        _ownerCanvas?.InvalidateConnections();
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

        PushMoveCommandIfMoved();

        _canvas = null;
        _ownerCanvas = null;
        _dragStartPositions.Clear();
        ReleaseMouseCapture();
    }

    private void PushMoveCommandIfMoved()
    {
        if (_ownerCanvas is null || _dragStartPositions.Count == 0)
            return;

        var moved = _dragStartPositions
            .Where(kvp => kvp.Value != kvp.Key.Position)
            .ToList();

        if (moved.Count == 0)
            return;

        if (moved.Count == 1)
        {
            var (node, from) = (moved[0].Key, moved[0].Value);
            _ownerCanvas.CommandManager.Push(new MoveNodeCommand(node, from, node.Position));
            return;
        }

        var composite = new CompositeCommand("Move Nodes");
        foreach (var (node, from) in moved)
            composite.Add(new MoveNodeCommand(node, from, node.Position));

        _ownerCanvas.CommandManager.Push(composite);
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

    public static readonly DependencyProperty ContentProperty =
    DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(NodeControl),
        new FrameworkPropertyMetadata(null));

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentTemplateProperty =
    DependencyProperty.Register(
        nameof(ContentTemplate),
        typeof(DataTemplate),
        typeof(NodeControl),
        new FrameworkPropertyMetadata(null));

    public DataTemplate? ContentTemplate
    {
        get => (DataTemplate?)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }
}

