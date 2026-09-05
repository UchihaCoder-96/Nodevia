using Nodevia.Models;
using System.ComponentModel;
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
            new FrameworkPropertyMetadata(null, OnPortChanged));

    public Port? Port
    {
        get => (Port?)GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }

    private static void OnPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PortControl)d;

        if (e.OldValue is Port oldPort)
            oldPort.PropertyChanged -= control.OnPortPropertyChanged;

        if (e.NewValue is Port newPort)
        {
            newPort.PropertyChanged += control.OnPortPropertyChanged;
            control.IsConnected = newPort.IsConnected;
        }
        else
        {
            control.IsConnected = false;
        }
    }

    private void OnPortPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Models.Port.IsConnected) && Port is not null)
            IsConnected = Port.IsConnected;
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
}

