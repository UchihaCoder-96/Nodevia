using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Nodevia.Models;

public class Node : INotifyPropertyChanged
{
    private string _title = "Node";
    private Point _position;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

