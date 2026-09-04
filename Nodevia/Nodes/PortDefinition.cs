using Nodevia.Models;

namespace Nodevia.Nodes;

public class PortDefinition
{
    public string Name { get; }
    public PortDirection Direction { get; }
    public string DataType { get; }

    public PortDefinition(
        string name,
        PortDirection direction,
        string dataType = "object")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Port name cannot be empty.", nameof(name));

        Name = name;
        Direction = direction;
        DataType = dataType;
    }
}

