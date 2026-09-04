using Nodevia.Models;

namespace Nodevia.Commands;

public class DeleteNodeCommand : INodeCommand
{
    private readonly NodeGraph _graph;
    private readonly Node _node;

    private readonly List<Connection> _connections = new();

    public string Name => "Delete Node";

    public DeleteNodeCommand(NodeGraph graph, Node node)
    {
        _graph = graph;
        _node = node;
    }

    public void Execute()
    {
        if (!_graph.Nodes.Contains(_node))
            return;

        // Only capture the connections the first time.
        if (_connections.Count == 0)
        {
            _connections.AddRange(
                _graph.Connections.Where(c =>
                    ReferenceEquals(c.Source.Owner, _node) ||
                    ReferenceEquals(c.Target.Owner, _node)));
        }

        _graph.Nodes.Remove(_node);
    }

    public void Undo()
    {
        if (!_graph.Nodes.Contains(_node))
            _graph.Nodes.Add(_node);

        foreach (var connection in _connections)
        {
            if (!_graph.Connections.Contains(connection))
                _graph.Connections.Add(connection);
        }
    }
}

