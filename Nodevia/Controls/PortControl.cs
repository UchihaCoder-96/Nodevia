using Nodevia.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

    public bool HasDefaultValue => Port?.Direction == PortDirection.Input && Port.DefaultValue is not null;

    public Point? GetCenterRelativeTo(FrameworkElement ancestor)
    {
        if (!IsLoaded || !ancestor.IsLoaded)
            return null;

        try
        {
            var transform = TransformToAncestor(ancestor);

            return transform.Transform(
                new Point(ActualWidth / 2, ActualHeight / 2));
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (Port is null)
            return;

        var ownerCanvas = FindAncestor<NodeCanvas>(this);
        if (ownerCanvas is null)
            return;

        ownerCanvas.BeginConnectionDrag(this);
        e.Handled = true;
    }

    private static T? FindAncestor<T>(DependencyObject start) where T : DependencyObject
    {
        DependencyObject? current = VisualTreeHelper.GetParent(start);
        while (current is not null)
        {
            if (current is T match)
                return match;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    public PortControl()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        FindAncestor<NodeCanvas>(this)?.RegisterPortControl(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        FindAncestor<NodeCanvas>(this)?.UnregisterPortControl(this);
    }

    public static readonly DependencyProperty IsConnectedProperty =
    DependencyProperty.Register(
        nameof(IsConnected),
        typeof(bool),
        typeof(PortControl),
        new FrameworkPropertyMetadata(false));

    public bool IsConnected
    {
        get => (bool)GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }

    public static readonly DependencyProperty IsHoveredProperty =
        DependencyProperty.Register(
            nameof(IsHovered),
            typeof(bool),
            typeof(PortControl),
            new FrameworkPropertyMetadata(false));

    public bool IsHovered
    {
        get => (bool)GetValue(IsHoveredProperty);
        set => SetValue(IsHoveredProperty, value);
    }
}

