using Nodevia.Commands;
using Nodevia.Models;
using Nodevia.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.UI;

public static class NodeMenuBuilder
{
    public static ContextMenu Build(
        NodeCatalog catalog,
        NodeFactory factory,
        Nodevia.Commands.CommandManager commandManager,
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
                    Node node = factory.Create(definition, canvasPosition);

                    commandManager.Execute(new AddNodeCommand(graph, node));
                };

                categoryItem.Items.Add(nodeItem);
            }

            menu.Items.Add(categoryItem);
        }

        return menu;
    }
}

