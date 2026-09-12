using Nodevia.Models;
using System.Windows;

namespace Nodevia.Commands;

internal sealed class NodeBlueprint
{
    public required string Title { get; init; }
    public required Point Position { get; init; }
    public string? DefinitionId { get; init; }
    public Execution.NodeBehavior? Behavior { get; init; }
    public string? BodyTemplateKey { get; init; }
    public required IReadOnlyList<Port> InputPorts { get; init; }
    public required IReadOnlyList<Port> OutputPorts { get; init; }
}

internal sealed class ConnectionBlueprint
{
    public required int SourceNodeIndex { get; init; }
    public required string SourcePortName { get; init; }
    public required int TargetNodeIndex { get; init; }
    public required string TargetPortName { get; init; }
}

public sealed class NodeClipboard
{
    private List<NodeBlueprint> _nodes = new();
    private List<ConnectionBlueprint> _connections = new();

    public bool HasContent => _nodes.Count > 0;

    public void Copy(IEnumerable<Node> nodes, IEnumerable<Connection> allConnections)
    {
        var nodeList = nodes.ToList();
        var indexByNode = new Dictionary<Node, int>();
        for (int i = 0; i < nodeList.Count; i++)
            indexByNode[nodeList[i]] = i;

        _nodes = nodeList.Select(n => new NodeBlueprint
        {
            Title = n.Title,
            Position = n.Position,
            DefinitionId = n.DefinitionId,
            Behavior = n.Behavior,
            BodyTemplateKey = n.BodyTemplateKey,
            InputPorts = n.InputPorts.Select(p => p.Clone()).ToList(),
            OutputPorts = n.OutputPorts.Select(p => p.Clone()).ToList()
        }).ToList();

        _connections = allConnections
            .Where(c => c.Source.Owner is Node sn && indexByNode.ContainsKey(sn)
                     && c.Target.Owner is Node tn && indexByNode.ContainsKey(tn))
            .Select(c => new ConnectionBlueprint
            {
                SourceNodeIndex = indexByNode[(Node)c.Source.Owner!],
                SourcePortName = c.Source.Name,
                TargetNodeIndex = indexByNode[(Node)c.Target.Owner!],
                TargetPortName = c.Target.Name
            }).ToList();
    }

    public (List<Node> nodes, CompositeCommand command) Instantiate(NodeGraph graph, Vector offset)
    {
        var newNodes = _nodes.Select(bp =>
        {
            var node = new Node
            {
                Title = bp.Title,
                Position = new Point(bp.Position.X + offset.X, bp.Position.Y + offset.Y),
                DefinitionId = bp.DefinitionId,
                Behavior = bp.Behavior,
                BodyTemplateKey = bp.BodyTemplateKey
            };

            foreach (var p in bp.InputPorts) node.InputPorts.Add(p.Clone());
            foreach (var p in bp.OutputPorts) node.OutputPorts.Add(p.Clone());

            return node;
        }).ToList();

        var command = new CompositeCommand("Paste");

        foreach (var node in newNodes)
            command.Add(new AddNodeCommand(graph, node));

        foreach (var cb in _connections)
        {
            var sourcePort = newNodes[cb.SourceNodeIndex].OutputPorts.First(p => p.Name == cb.SourcePortName);
            var targetPort = newNodes[cb.TargetNodeIndex].InputPorts.First(p => p.Name == cb.TargetPortName);

            command.Add(new AddConnectionCommand(graph, new Connection(sourcePort, targetPort)));
        }

        return (newNodes, command);
    }
}

