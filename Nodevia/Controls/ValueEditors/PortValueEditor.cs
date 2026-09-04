using Nodevia.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls.ValueEditors;

public abstract class PortValueEditor : Control
{
    public static readonly DependencyProperty PortProperty =
        DependencyProperty.Register(
            nameof(Port),
            typeof(Port),
            typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnPortChanged));

    public Port? Port
    {
        get => (Port?)GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnValueChanged));

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    // Guards against Port -> Value -> Port feedback while syncing.
    private bool _isSyncing;

    private static void OnPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (PortValueEditor)d;

        if (e.OldValue is Port oldPort)
            oldPort.PropertyChanged -= editor.OnPortPropertyChanged;

        if (e.NewValue is Port newPort)
        {
            newPort.PropertyChanged += editor.OnPortPropertyChanged;
            editor.SyncValueFromPort(newPort);
        }
    }

    private void OnPortPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Models.Port.DefaultValue) && Port is not null)
            SyncValueFromPort(Port);
    }

    private void SyncValueFromPort(Port port)
    {
        _isSyncing = true;
        Value = port.DefaultValue;
        _isSyncing = false;
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (PortValueEditor)d;

        if (editor._isSyncing || editor.Port is null)
            return;

        editor.Port.DefaultValue = e.NewValue;
    }
}

