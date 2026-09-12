using Nodevia.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace Nodevia.Models
{
    public class NodeGraph
    {
        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Connection> Connections { get; } = new();

        public event EventHandler? ValueChanged;
        public bool LiveUpdate { get; set; } = true;
        public GraphLog Log { get; } = new();

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

            if (LiveUpdate) RefreshLiveValues();
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

            if (!AreDataTypesCompatible(source.DataType, target.DataType))
                throw new InvalidOperationException(
                    $"Cannot connect '{source.DataType}' output to '{target.DataType}' input - incompatible data types.");

            if (source.Owner is Node sourceNode && target.Owner is Node targetNode &&
                WouldCreateCycle(sourceNode, targetNode))
            {
                throw new InvalidOperationException("This connection would create a cycle.");
            }

            return new Connection(source, target);
        }

        private static bool AreDataTypesCompatible(string sourceType, string targetType)
        {
            if (IsUniversalType(sourceType) || IsUniversalType(targetType))
                return true;

            return string.Equals(sourceType, targetType, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUniversalType(string dataType) =>
            string.Equals(dataType, "object", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(dataType, "none", StringComparison.OrdinalIgnoreCase);

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
            if (e.NewItems is not null)
            {
                foreach (Node node in e.NewItems)
                    SubscribePorts(node);
            }

            if (e.OldItems is not null)
            {
                foreach (Node node in e.OldItems)
                {
                    UnsubscribePorts(node);

                    var toRemove = Connections
                        .Where(c => ReferenceEquals(c.Source.Owner, node) || ReferenceEquals(c.Target.Owner, node))
                        .ToList();

                    foreach (var connection in toRemove)
                        Connections.Remove(connection);
                }
            }

            if (LiveUpdate) RefreshLiveValues();
        }

        private void SubscribePorts(Node node)
        {
            foreach (var port in node.InputPorts.Concat(node.OutputPorts))
                port.PropertyChanged += OnPortPropertyChanged;
        }

        private void UnsubscribePorts(Node node)
        {
            foreach (var port in node.InputPorts.Concat(node.OutputPorts))
                port.PropertyChanged -= OnPortPropertyChanged;
        }

        private void OnPortPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Port.DefaultValue))
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
                if (LiveUpdate) RefreshLiveValues();
            }
        }

        private void RefreshLiveValues()
        {
            var evaluator = new Execution.GraphEvaluator(this);

            foreach (var node in Nodes)
            {
                foreach (var port in node.InputPorts)
                {
                    if (!port.IsConnected)
                        continue;

                    try
                    {
                        port.LiveDisplayValue = evaluator.GetInputValue(node, port.Name);
                    }
                    catch (Exception ex)
                    {
                        port.LiveDisplayValue = null;
                        Log.Error($"'{node.Title}' input '{port.Name}': {ex.Message}");
                    }
                }

                if (node.Behavior is not null)
                {
                    try
                    {
                        node.Outputs = evaluator.Evaluate(node).Values;
                    }
                    catch (Exception ex)
                    {
                        node.Outputs = node.OutputPorts.ToDictionary(p => p.Name, p => (object?)null);
                        Log.Error($"'{node.Title}': {ex.Message}");
                    }
                }
            }
        }

        public void Refresh() => RefreshLiveValues();
    }
}
