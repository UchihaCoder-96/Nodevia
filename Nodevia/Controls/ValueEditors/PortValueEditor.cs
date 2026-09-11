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
        else
        {
            editor.DisplayValue = null;
        }

        editor.OnPortAttached(e.NewValue as Port);
    }

    private void OnPortPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Port is null)
            return;

        if (e.PropertyName == nameof(Port.DefaultValue))
        {
            SyncValueFromPort(Port);
            return;
        }

        if (e.PropertyName == nameof(Port.LiveDisplayValue))
        {
            ApplyLiveValue();
            return;
        }

        if (e.PropertyName == nameof(Port.IsConnected))
        {
            RefreshDisplayValue();
        }
    }

    private void SyncValueFromPort(Port port)
    {
        _isSyncing = true;

        Value = port.DefaultValue;

        _isSyncing = false;

        RefreshDisplayValue();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (PortValueEditor)d;

        if (!editor._isSyncing && editor.Port is not null)
            editor.Port.DefaultValue = e.NewValue;

        editor.OnValueChanged();
        editor.RefreshDisplayValue();
    }

    private void ApplyLiveValue()
    {
        if (Port is null || !Port.IsConnected)
            return;

        _isSyncing = true;

        Value = Port.LiveDisplayValue;
        Port.DefaultValue = Port.LiveDisplayValue;

        _isSyncing = false;

        DisplayValue = Value;
    }

    private void RefreshDisplayValue()
    {
        if (Port is { IsConnected: true })
        {
            
            DisplayValue = Port.LiveDisplayValue ?? Value;
        }
        else
        {
            DisplayValue = Value;
        }
    }

    public static readonly DependencyProperty DisplayValueProperty =
        DependencyProperty.Register(
            nameof(DisplayValue),
            typeof(object),
            typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnDisplayValueChanged));

    public object? DisplayValue
    {
        get => GetValue(DisplayValueProperty);
        private set => SetValue(DisplayValueProperty, value);
    }

    private static void OnDisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PortValueEditor)d).OnDisplayValueChanged();
    }

    protected virtual void OnDisplayValueChanged() { }
    protected virtual void OnValueChanged() { }
    protected virtual void OnPortAttached(Port? port) { }
}


