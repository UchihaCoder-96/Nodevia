using Nodevia.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class NodeCanvas : ItemsControl
{
    static NodeCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NodeCanvas),
            new FrameworkPropertyMetadata(typeof(NodeCanvas)));
    }

    public ObservableCollection<Node> Nodes { get; } = new();

    public NodeCanvas()
    {
        ItemsSource = Nodes;
    }
}

