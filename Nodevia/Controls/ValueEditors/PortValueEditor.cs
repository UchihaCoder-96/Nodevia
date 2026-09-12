using Nodevia.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls.ValueEditors;

public abstract class PortValueEditor : Control
{
    public static readonly DependencyProperty PortProperty =
        DependencyProperty.Register(nameof(Port), typeof(Port), typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnPortChanged));

    public Port? Port
    {
        get => (Port?)GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnValueChanged));

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty DisplayValueProperty =
        DependencyProperty.Register(nameof(DisplayValue), typeof(object), typeof(PortValueEditor),
            new FrameworkPropertyMetadata(null, OnDisplayValueChanged));

    public object? DisplayValue
    {
        get => GetValue(DisplayValueProperty);
        private set => SetValue(DisplayValueProperty, value);
    }

    private bool _isSyncing;

    private object? _lastLiveValue;

    private static void OnPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (PortValueEditor)d;

        if (e.OldValue is Port oldPort)
            oldPort.PropertyChanged -= editor.OnPortPropertyChanged;

        editor._lastLiveValue = null;

        if (e.NewValue is Port newPort)
        {
            newPort.PropertyChanged += editor.OnPortPropertyChanged;
            editor.SyncValueFromPort(newPort);

            if (newPort.IsConnected)
                editor._lastLiveValue = newPort.LiveDisplayValue;
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

        switch (e.PropertyName)
        {
            case nameof(Models.Port.DefaultValue):
                SyncValueFromPort(Port);
                break;

            case nameof(Models.Port.LiveDisplayValue):
                if (Port.IsConnected)
                {
                    _lastLiveValue = Port.LiveDisplayValue;
                    RefreshDisplayValue();
                }
                break;

            case nameof(Models.Port.IsConnected):
                HandleConnectionStateChanged();
                break;
        }
    }

    private void HandleConnectionStateChanged()
    {
        if (Port is null)
            return;

        if (!Port.IsConnected)
        {
            if (_lastLiveValue is not null)
            {
                Value = _lastLiveValue;
                _lastLiveValue = null;
            }
        }

        RefreshDisplayValue();
    }

    private void SyncValueFromPort(Port port)
    {
        _isSyncing = true;
        Value = port.DefaultValue;
        _isSyncing = false;

        RefreshDisplayValue();
    }

    private void RefreshDisplayValue()
    {
        DisplayValue = Port is { IsConnected: true } ? Port.LiveDisplayValue : Value;
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (PortValueEditor)d;

        if (!editor._isSyncing && editor.Port is not null)
            editor.Port.DefaultValue = e.NewValue;

        editor.RefreshDisplayValue();
    }

    private static void OnDisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PortValueEditor)d).OnDisplayValueChanged();
    }

    protected virtual void OnDisplayValueChanged() { }
    protected virtual void OnPortAttached(Port? port) { }
}

