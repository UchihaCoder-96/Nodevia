namespace Nodevia.Serialization;

public class GraphDto
{
    public List<NodeDto> Nodes { get; set; } = new();
    public List<ConnectionDto> Connections { get; set; } = new();
}

public class NodeDto
{
    public required string InstanceId { get; set; } // this Node's own Guid as a string, lets ConnectionDto reference it
    public required string DefinitionId { get; set; } // NodeDefinition.Id, looked up in the catalog on load
    public required string Title { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    public int ZIndex { get; set; }
    public Dictionary<string, object?> InputValues { get; set; } = new(); // port name -> DefaultValue
}

public class ConnectionDto
{
    public required string SourceNodeId { get; set; }
    public required string SourcePortName { get; set; }
    public required string TargetNodeId { get; set; }
    public required string TargetPortName { get; set; }
}

