using System.Windows;
using Nodevia.Models;

namespace Nodevia.Nodes;

public class NodeFactory
{
    public Node Create(NodeDefinition definition, Point position)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var node = new Node
        {
            Title = definition.Title,
            Position = position,
            DefinitionId = definition.Id,
            Behavior = definition.Behavior,
            BodyTemplateKey = definition.BodyTemplateKey,
            Outputs = definition.Outputs.ToDictionary(o => o.Name, o => (object?)null)
        };

        foreach (var port in definition.Inputs)
        {
            var nodePort = new Port(
                port.Name,
                PortDirection.Input,
                port.DataType,
                port.DefaultValue);

            nodePort.EnumValues = port.EnumValues;
            nodePort.Metadata = port.Metadata;
            nodePort.AllowConnections = port.AllowConnections;

            node.InputPorts.Add(nodePort);
        }

        foreach (var port in definition.Outputs)
        {
            var nodePort = new Port(
                port.Name,
                PortDirection.Output,
                port.DataType,
                port.DefaultValue);

            nodePort.EnumValues = port.EnumValues;
            nodePort.Metadata = port.Metadata;

            node.OutputPorts.Add(nodePort);
        }

        return node;
    }
}

