using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls
{
    public class NodeControl : Control
    {
        static NodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(NodeControl),
                new FrameworkPropertyMetadata(typeof(NodeControl)));
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(NodeControl),
                new FrameworkPropertyMetadata("Node"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}

