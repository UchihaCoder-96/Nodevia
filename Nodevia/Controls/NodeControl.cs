using System.Windows;
using System.Windows.Controls;
using Nodevia.Models;

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
}

