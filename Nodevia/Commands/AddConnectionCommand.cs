using Nodevia.Models;

namespace Nodevia.Commands;

public class AddConnectionCommand : INodeCommand
{
    private readonly NodeGraph _graph;
    private readonly Connection _connection;

    public string Name => "Add Connection";

    public AddConnectionCommand(
        NodeGraph graph,
        Connection connection)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(connection);

        _graph = graph;
        _connection = connection;
    }

    public void Execute()
    {
        if (!_graph.Connections.Contains(_connection))
            _graph.Connections.Add(_connection);
    }

    public void Undo()
    {
        _graph.Connections.Remove(_connection);
    }
}

