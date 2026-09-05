using Nodevia.Models;

namespace Nodevia.Nodes;

public class PortDefinition
{
    public string Name { get; }
    public PortDirection Direction { get; }
    public string DataType { get; }
    public object? DefaultValue { get; }
    public IReadOnlyList<string> EnumValues { get; }
    public IReadOnlyDictionary<string, object> Metadata { get; }

    public PortDefinition(
        string name,
        PortDirection direction,
        string dataType = "object",
        object? defaultValue = null,
        IEnumerable<string>? enumValues = null,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Port name cannot be empty.", nameof(name));

        Name = name;
        Direction = direction;
        DataType = dataType;
        EnumValues = enumValues?.ToList() ?? [];
        DefaultValue = CoerceDefaultValue(dataType, defaultValue, Name);
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    private static object? CoerceDefaultValue(string dataType, object? value, string portName)
    {
        if (value is null)
            return null;

        switch (dataType.ToLowerInvariant())
        {
            case "int":
                if (value is int)
                    return value;
                if (value is string s && int.TryParse(s, out int i))
                    return i;
                throw new ArgumentException(
                    $"Port '{portName}': default value '{value}' is not a valid int.", nameof(value));

            case "float":
                if (value is double or float)
                    return Convert.ToDouble(value);
                if (value is string s2 && double.TryParse(s2, out double d))
                    return d;
                throw new ArgumentException(
                    $"Port '{portName}': default value '{value}' is not a valid float.", nameof(value));

            case "bool":
                if (value is bool)
                    return value;
                if (value is string s3 && bool.TryParse(s3, out bool b))
                    return b;
                throw new ArgumentException(
                    $"Port '{portName}': default value '{value}' is not a valid bool.", nameof(value));

            default:
                return value;
        }
    }
}

