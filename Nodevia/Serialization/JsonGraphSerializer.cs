using Nodevia.Models;
using Nodevia.Nodes;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace Nodevia.Serialization;

public class JsonGraphSerializer : IGraphSerializer
{
    private static readonly Dictionary<string, Type> KnownTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["int"] = typeof(int),
        ["float"] = typeof(double),
        ["bool"] = typeof(bool),
        ["string"] = typeof(string),
        ["vec2"] = typeof(Vec2),
        ["vec3"] = typeof(Vec3),
        ["color"] = typeof(System.Windows.Media.Color),
    };

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public string Serialize(NodeGraph graph)
    {
        var nodeIds = new Dictionary<Node, string>();
        foreach (var node in graph.Nodes)
            nodeIds[node] = node.Id.ToString();

        var dto = new GraphDto
        {
            Nodes = graph.Nodes.Select(n => new NodeDto
            {
                InstanceId = nodeIds[n],
                DefinitionId = n.DefinitionId
                    ?? throw new InvalidOperationException(
                        $"Node '{n.Title}' has no DefinitionId and cannot be serialized. " +
                        "Nodes must be created via NodeFactory.Create."),
                Title = n.Title,
                X = n.Position.X,
                Y = n.Position.Y,
                Subtitle = n.Subtitle,
                ZIndex = n.ZIndex,
                InputValues = n.InputPorts.ToDictionary(p => p.Name, p => p.DefaultValue)
            }).ToList(),

            Connections = graph.Connections.Select(c => new ConnectionDto
            {
                SourceNodeId = nodeIds[(Node)c.Source.Owner!],
                SourcePortName = c.Source.Name,
                TargetNodeId = nodeIds[(Node)c.Target.Owner!],
                TargetPortName = c.Target.Name
            }).ToList()
        };

        return JsonSerializer.Serialize(dto, Options);
    }

    public NodeGraph Deserialize(string data, NodeCatalog catalog, NodeFactory factory)
    {
        var dto = JsonSerializer.Deserialize<GraphDto>(data, Options)
            ?? throw new InvalidDataException("Save data could not be parsed.");

        var graph = new NodeGraph();
        var nodesByInstanceId = new Dictionary<string, Node>();

        foreach (var nodeDto in dto.Nodes)
        {
            var definition = catalog.Get(nodeDto.DefinitionId);
            var node = factory.Create(definition, new Point(nodeDto.X, nodeDto.Y));

            node.Title = nodeDto.Title;
            node.Subtitle = nodeDto.Subtitle;
            node.ZIndex = nodeDto.ZIndex;

            foreach (var port in node.InputPorts)
            {
                if (nodeDto.InputValues.TryGetValue(port.Name, out var savedValue))
                    port.DefaultValue = CoerceValue(savedValue, port.DataType, port.DefaultValue);
            }

            graph.Nodes.Add(node);
            nodesByInstanceId[nodeDto.InstanceId] = node;
        }

        foreach (var connDto in dto.Connections)
        {
            if (!nodesByInstanceId.TryGetValue(connDto.SourceNodeId, out var sourceNode) ||
                !nodesByInstanceId.TryGetValue(connDto.TargetNodeId, out var targetNode))
                continue; // referenced node missing, skip this link

            var sourcePort = sourceNode.OutputPorts.FirstOrDefault(p => p.Name == connDto.SourcePortName);
            var targetPort = targetNode.InputPorts.FirstOrDefault(p => p.Name == connDto.TargetPortName);

            if (sourcePort is null || targetPort is null)
                continue; // port renamed/removed since save, skip this link rather than throwing

            try
            {
                var connection = graph.CreateConnection(sourcePort, targetPort);
                graph.Connections.Add(connection);
            }
            catch (InvalidOperationException)
            {
                // Save data referenced an invalid link (shouldnt normally happen) soo... skip it.
            }
        }

        return graph;
    }

    private static object? CoerceValue(object? savedValue, string dataType, object? currentDefault)
    {
        if (savedValue is not JsonElement element)
            return savedValue;

        switch (dataType.ToLowerInvariant())
        {
            case "int":
                return element.GetInt32();

            case "float":
                return element.GetDouble();

            case "bool":
                return element.GetBoolean();

            case "string":
            case "enum":
                return element.GetString();

            case "vec2":
                return new Vec2(
                    GetDouble(element, "X"),
                    GetDouble(element, "Y"));

            case "vec3":
                return new Vec3(
                    GetDouble(element, "X"),
                    GetDouble(element, "Y"),
                    GetDouble(element, "Z"));

            case "color":
                return Color.FromScRgb(
                    (float)GetDouble(element, "ScA"),
                    (float)GetDouble(element, "ScR"),
                    (float)GetDouble(element, "ScG"),
                    (float)GetDouble(element, "ScB"));

            default:
                if (currentDefault is null)
                    return currentDefault;

                try
                {
                    return JsonSerializer.Deserialize(element.GetRawText(), currentDefault.GetType(), Options);
                }
                catch
                {
                    return currentDefault;
                }
        }
    }

    private static double GetDouble(JsonElement element, string propertyName)
    {
        foreach (var prop in element.EnumerateObject())
        {
            if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                return prop.Value.GetDouble();
        }

        return 0.0;
    }
}

