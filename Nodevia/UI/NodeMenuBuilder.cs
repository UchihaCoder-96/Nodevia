using System.Windows;
using System.Windows.Controls;
using Nodevia.Models;
using Nodevia.Nodes;

namespace Nodevia.UI;

public static class NodeMenuBuilder
{
    public static ContextMenu Build(
        NodeCatalog catalog,
        NodeFactory factory,
        NodeGraph graph,
        Point canvasPosition)
    {
        var menu = new ContextMenu();

        foreach (string category in catalog.Categories)
        {
            var categoryItem = new MenuItem
            {
                Header = category
            };

            foreach (NodeDefinition definition in catalog.GetByCategory(category))
            {
                var nodeItem = new MenuItem
                {
                    Header = definition.Title,
                    Tag = definition
                };

                nodeItem.Click += (_, _) =>
                {
                    Node node = factory.Create(
                        definition,
                        canvasPosition);

                    graph.Nodes.Add(node);
                };

                categoryItem.Items.Add(nodeItem);
            }

            menu.Items.Add(categoryItem);
        }

        return menu;
    }
}

