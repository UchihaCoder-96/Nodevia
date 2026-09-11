using Nodevia.Models;

namespace Nodevia.Execution;

public class GraphEvaluator
{
    private readonly NodeGraph _graph;
    private readonly Dictionary<Node, NodeOutputs> _cache = new();
    private readonly HashSet<Node> _evaluating = new();

    public GraphEvaluator(NodeGraph graph)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public NodeOutputs Evaluate(Node node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (_cache.TryGetValue(node, out var cached))
            return cached;

        if (!_evaluating.Add(node))
            throw new InvalidOperationException(
                $"Cycle detected while evaluating node '{node.Title}'.");

        try
        {
            if (node.Behavior is null)
                throw new InvalidOperationException(
                    $"Node '{node.Title}' has no behavior assigned and cannot be evaluated.");

            var inputs = GatherInputs(node);
            var outputs = node.Behavior.Evaluate(inputs);

            _cache[node] = outputs;
            return outputs;
        }
        finally
        {
            _evaluating.Remove(node);
        }
    }

    private NodeInputs GatherInputs(Node node)
    {
        var values = new Dictionary<string, object?>();

        foreach (var port in node.InputPorts)
            values[port.Name] = ResolveInputValue(port);

        return new NodeInputs(values);
    }

    private object? ResolveInputValue(Port inputPort)
    {
        var connection = _graph.Connections.FirstOrDefault(c => c.Target == inputPort);

        if (connection is null)
            return inputPort.DefaultValue;

        if (connection.Source.Owner is not Node sourceNode)
            return inputPort.DefaultValue;

        var sourceOutputs = Evaluate(sourceNode);
        return sourceOutputs.Get(connection.Source.Name);
    }

    public object? GetInputValue(Node node, string portName)
    {
        var port = node.InputPorts.FirstOrDefault(p => p.Name == portName)
            ?? throw new ArgumentException($"Node '{node.Title}' has no input port named '{portName}'.", nameof(portName));

        return ResolveInputValue(port);
    }
}

