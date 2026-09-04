using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Nodevia.Models;

public class Node : INotifyPropertyChanged
{
    private string _title = "Node";
    private Point _position;
    private int _zIndex;
    private bool _isSelected;

    public Guid Id { get; } = Guid.NewGuid();

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value)
                return;

            _title = value;
            OnPropertyChanged();
        }
    }

    public Point Position
    {
        get => _position;
        set
        {
            if (_position == value)
                return;

            _position = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public int ZIndex
    {
        get => _zIndex;
        set
        {
            if (_zIndex == value)
                return;

            _zIndex = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Port> InputPorts { get; } = new();
    public ObservableCollection<Port> OutputPorts { get; } = new();

    public Node()
    {
        InputPorts.CollectionChanged += (_, e) => OnPortsChanged(e, PortDirection.Input);
        OutputPorts.CollectionChanged += (_, e) => OnPortsChanged(e, PortDirection.Output);
    }

    private void OnPortsChanged(NotifyCollectionChangedEventArgs e, PortDirection expected)
    {
        if (e.NewItems is not null)
        {
            foreach (Port port in e.NewItems)
            {
                if (port.Direction != expected)
                    throw new InvalidOperationException(
                        $"Cannot add a {port.Direction} port to {expected}Ports.");

                port.Owner = this;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (Port port in e.OldItems)
            {
                if (ReferenceEquals(port.Owner, this))
                    port.Owner = null;
            }
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

