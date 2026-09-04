using Nodevia.Models;
using System.Reflection.Metadata;

namespace Nodevia.Nodes;

public class NodeDefinition
{
    public string Id { get; }
    public string Title { get; }
    public string Category { get; }

    public IReadOnlyList<PortDefinition> Inputs { get; }
    public IReadOnlyList<PortDefinition> Outputs { get; }

    public NodeDefinition(
        string id,
        string title,
        string category,
        IEnumerable<PortDefinition>? inputs = null,
        IEnumerable<PortDefinition>? outputs = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Node definition ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Node definition title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Node definition category cannot be empty.", nameof(category));

        Id = id;
        Title = title;
        Category = category;

        Inputs = inputs?.ToList() ?? [];
        Outputs = outputs?.ToList() ?? [];
    }
}

