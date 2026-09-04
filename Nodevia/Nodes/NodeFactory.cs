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
            Position = position
        };

        foreach (var port in definition.Inputs)
        {
            node.InputPorts.Add(
                new Port(
                    port.Name,
                    PortDirection.Input,
                    port.DataType));
        }

        foreach (var port in definition.Outputs)
        {
            node.OutputPorts.Add(
                new Port(
                    port.Name,
                    PortDirection.Output,
                    port.DataType));
        }

        return node;
    }
}

