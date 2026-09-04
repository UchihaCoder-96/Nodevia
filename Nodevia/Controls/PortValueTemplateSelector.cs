using Nodevia.Models;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class PortValueTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not Port port || container is not FrameworkElement element)
            return null;

        string key = $"PortValueTemplate.{port.DataType.ToLowerInvariant()}";

        return element.TryFindResource(key) as DataTemplate
            ?? element.TryFindResource("PortValueTemplate.Default") as DataTemplate;
    }
}

