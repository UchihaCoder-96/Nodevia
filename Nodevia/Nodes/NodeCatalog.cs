namespace Nodevia.Nodes;

public class NodeCatalog
{
    private readonly Dictionary<string, NodeDefinition> _definitions = new();

    public IReadOnlyCollection<NodeDefinition> Definitions =>
        _definitions.Values;

    public IEnumerable<string> Categories =>
        _definitions.Values
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x);

    public void Register(NodeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (!_definitions.TryAdd(definition.Id, definition))
        {
            throw new InvalidOperationException(
                $"A node definition with ID '{definition.Id}' is already registered.");
        }
    }

    public bool Unregister(string id)
    {
        return _definitions.Remove(id);
    }

    public bool TryGet(string id, out NodeDefinition? definition)
    {
        return _definitions.TryGetValue(id, out definition);
    }

    public NodeDefinition Get(string id)
    {
        if (!_definitions.TryGetValue(id, out var definition))
        {
            throw new KeyNotFoundException(
                $"No node definition with ID '{id}' is registered.");
        }

        return definition;
    }

    public IEnumerable<NodeDefinition> GetByCategory(string category)
    {
        return _definitions.Values
            .Where(x => x.Category == category);
    }
}

