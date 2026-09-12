using Nodevia.Models;
using Nodevia.Nodes;

namespace Nodevia.Serialization;

public interface IGraphSerializer
{
    string Serialize(NodeGraph graph);

    NodeGraph Deserialize(string data, NodeCatalog catalog, NodeFactory factory);
}

