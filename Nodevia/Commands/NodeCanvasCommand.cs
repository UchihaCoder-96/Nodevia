using System.Windows.Input;

namespace Nodevia.Commands;

public static class NodeCanvasCommands
{
    public static readonly RoutedUICommand DeleteSelection = new(
        "Delete Selection", nameof(DeleteSelection), typeof(NodeCanvasCommands));

    public static readonly RoutedUICommand SelectAll = new(
        "Select All", nameof(SelectAll), typeof(NodeCanvasCommands));

    public static readonly RoutedUICommand CancelAction = new(
        "Cancel Action", nameof(CancelAction), typeof(NodeCanvasCommands));
}

