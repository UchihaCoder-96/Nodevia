using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Nodevia.Models
{
    public class NodeGraph
    {
        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Connection> Connections { get; } = new();

        public NodeGraph()
        {
            Nodes.CollectionChanged += OnNodesChanged;
        }

        public Connection Connect(Port source, Port target)
        {
            var connection = CreateConnection(source, target);
            Connections.Add(connection);
            return connection;
        }

        public Connection CreateConnection(Port source, Port target)
        {
            if (source.Direction != PortDirection.Output)
                throw new ArgumentException(
                    "Source must be an Output port.", nameof(source));

            if (target.Direction != PortDirection.Input)
                throw new ArgumentException(
                    "Target must be an Input port.", nameof(target));

            if (ReferenceEquals(source.Owner, target.Owner))
                throw new InvalidOperationException(
                    "Cannot connect a node to itself.");

            if (Connections.Any(c => c.Target == target))
                throw new InvalidOperationException(
                    "Target port already has a connection.");

            return new Connection(source, target);
        }

        public void Disconnect(Connection connection) => Connections.Remove(connection);

        private void OnNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is null)
                return;

            foreach (Node node in e.OldItems)
            {
                var toRemove = Connections
                    .Where(c => ReferenceEquals(c.Source.Owner, node) || ReferenceEquals(c.Target.Owner, node))
                    .ToList();

                foreach (var connection in toRemove)
                    Connections.Remove(connection);
            }
        }
    }
}
