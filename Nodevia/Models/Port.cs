using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nodevia.Models
{
    public enum PortDirection
    {
        Input,
        Output
    }

    public class Port : INotifyPropertyChanged
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Node? Owner { get; internal set; }

        public PortDirection Direction { get; }

        private string _name = "Port";
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        // Loosely typed for now (eg. float, int, exec) Can evolve into a richer PortDataType later
        private string _dataType = "object";
        public string DataType
        {
            get => _dataType;
            set
            {
                if (_dataType == value)
                    return;

                _dataType = value;
                OnPropertyChanged();
            }
        }

        public Port(string name, PortDirection direction, string dataType = "object")
        {
            _name = name;
            Direction = direction;
            _dataType = dataType;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
