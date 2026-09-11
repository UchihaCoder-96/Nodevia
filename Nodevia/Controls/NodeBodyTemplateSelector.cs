using Nodevia.Models;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class NodeBodyTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not Node node || string.IsNullOrEmpty(node.BodyTemplateKey) || container is not FrameworkElement element)
            return null;

        return element.TryFindResource($"NodeBody.{node.BodyTemplateKey}") as DataTemplate;
    }
}

