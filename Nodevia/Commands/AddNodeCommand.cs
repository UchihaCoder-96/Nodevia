using Nodevia.Models;

namespace Nodevia.Commands;

public class AddNodeCommand : INodeCommand
{
    private readonly NodeGraph _graph;
    private readonly Node _node;

    public string Name => "Add Node";

    public AddNodeCommand(NodeGraph graph, Node node)
    {
        _graph = graph;
        _node = node;
    }

    public void Execute()
    {
        if (!_graph.Nodes.Contains(_node))
            _graph.Nodes.Add(_node);
    }

    public void Undo()
    {
        _graph.Nodes.Remove(_node);
    }
}

