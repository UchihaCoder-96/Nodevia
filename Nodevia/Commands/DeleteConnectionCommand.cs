using Nodevia.Models;

namespace Nodevia.Commands;

public class DeleteConnectionCommand : INodeCommand
{
    private readonly NodeGraph _graph;
    private readonly Connection _connection;

    public string Name => "Delete Connection";

    public DeleteConnectionCommand(
        NodeGraph graph,
        Connection connection)
    {
        _graph = graph;
        _connection = connection;
    }

    public void Execute()
    {
        _graph.Disconnect(_connection);
    }

    public void Undo()
    {
        if (!_graph.Connections.Contains(_connection))
            _graph.Connections.Add(_connection);
    }
}

