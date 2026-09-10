namespace Nodevia.Execution;

public sealed class NodeInputs
{
    private readonly IReadOnlyDictionary<string, object?> _values;

    internal NodeInputs(IReadOnlyDictionary<string, object?> values)
    {
        _values = values;
    }

    public T Get<T>(string portName)
    {
        if (!_values.TryGetValue(portName, out var value))
            throw new ArgumentException($"No input named '{portName}' was provided.", nameof(portName));

        if (value is null)
        {
            if (default(T) is null)
                return default!;

            throw new InvalidOperationException(
                $"Input '{portName}' is null but {typeof(T).Name} is not nullable.");
        }

        if (value is T typed)
            return typed;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            throw new InvalidOperationException(
                $"Input '{portName}' is of type {value.GetType().Name}, expected {typeof(T).Name}.");
        }
    }

    public object? GetRaw(string portName) =>
        _values.TryGetValue(portName, out var value) ? value : null;
}

