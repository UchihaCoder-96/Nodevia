using Nodevia.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class LogPanel : Control
{
    static LogPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(LogPanel),
            new FrameworkPropertyMetadata(typeof(LogPanel)));
    }

    public static readonly DependencyProperty LogProperty =
        DependencyProperty.Register(nameof(Log), typeof(GraphLog), typeof(LogPanel),
            new FrameworkPropertyMetadata(null));

    public GraphLog? Log
    {
        get => (GraphLog?)GetValue(LogProperty);
        set => SetValue(LogProperty, value);
    }
}

