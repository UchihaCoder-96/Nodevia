using System;
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

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

