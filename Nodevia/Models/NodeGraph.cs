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
            Connections.CollectionChanged += OnConnectionsChanged;
        }

        private void OnConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var affectedPorts = new HashSet<Port>();

            if (e.OldItems is not null)
                foreach (Connection c in e.OldItems)
                {
                    affectedPorts.Add(c.Source);
                    affectedPorts.Add(c.Target);
                }

            if (e.NewItems is not null)
                foreach (Connection c in e.NewItems)
                {
                    affectedPorts.Add(c.Source);
                    affectedPorts.Add(c.Target);
                }

            foreach (var port in affectedPorts)
                port.IsConnected = Connections.Any(c => c.Source == port || c.Target == port);
        }

        public Connection CreateConnection(Port source, Port target)
        {
            if (source.Direction != PortDirection.Output)
                throw new ArgumentException("Source must be an Output port.", nameof(source));

            if (target.Direction != PortDirection.Input)
                throw new ArgumentException("Target must be an Input port.", nameof(target));

            if (ReferenceEquals(source.Owner, target.Owner))
                throw new InvalidOperationException("Cannot connect a node to itself.");

            if (Connections.Any(c => c.Target == target))
                throw new InvalidOperationException("Target port already has a connection.");

            if (source.Owner is Node sourceNode && target.Owner is Node targetNode &&
                WouldCreateCycle(sourceNode, targetNode))
            {
                throw new InvalidOperationException("This connection would create a cycle.");
            }

            return new Connection(source, target);
        }

        private bool WouldCreateCycle(Node sourceNode, Node targetNode)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            stack.Push(targetNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current == sourceNode)
                    return true;

                if (!visited.Add(current))
                    continue;

                foreach (var connection in Connections)
                {
                    if (connection.Source.Owner == current && connection.Target.Owner is Node next)
                        stack.Push(next);
                }
            }

            return false;
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
