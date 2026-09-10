namespace Nodevia.Execution;

public sealed class NodeOutputs
{
    private readonly Dictionary<string, object?> _values = new();

    public void Set(string portName, object? value) => _values[portName] = value;

    public object? Get(string portName)
    {
        if (!_values.TryGetValue(portName, out var value))
            throw new KeyNotFoundException(
                $"No output named '{portName}' was set by this node's behavior.");

        return value;
    }
}

