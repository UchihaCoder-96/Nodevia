using Nodevia.Models;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class PortControl : Control
{
    static PortControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PortControl),
            new FrameworkPropertyMetadata(typeof(PortControl)));
    }

    public static readonly DependencyProperty PortProperty =
        DependencyProperty.Register(
            nameof(Port),
            typeof(Port),
            typeof(PortControl),
            new FrameworkPropertyMetadata(null));

    public Port? Port
    {
        get => (Port?)GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }
}

