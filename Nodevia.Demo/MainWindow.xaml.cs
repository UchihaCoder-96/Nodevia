using Nodevia.Commands;
using Nodevia.Controls;
using Nodevia.Models;
using Nodevia.Nodes;
using Nodevia.UI;
using System.Windows;
using System.Windows.Input;

namespace Nodevia.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NodeCatalog _catalog = new();
        private readonly NodeFactory _factory = new();

        public MainWindow()
        {
            InitializeComponent();

            RegisterNodes();
            AddDemoNodes();

            NodeCanvas.MouseRightButtonUp += OnCanvasRightClick;
        }

        private void RegisterNodes()
        {
            _catalog.Register(
                new NodeDefinition(
                    id: "math.add",
                    title: "Add",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 5f),
                        new PortDefinition("B", PortDirection.Input, "float", 10f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.subtract",
                    title: "Subtract",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                        new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.multiply",
                    title: "Multiply",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.divide",
                    title: "Divide",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.modulo",
                    title: "Modulo",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.power",
                    title: "Power",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("Base", PortDirection.Input, "float", 0f),
                new PortDefinition("Exponent", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.negate",
                    title: "Negate",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("Value", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.absolute",
                    title: "Absolute",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("Value", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.sqrt",
                    title: "Square Root",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("Value", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.min",
                    title: "Min",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "math.max",
                    title: "Max",
                    category: "Math",
                    inputs:
                    [
                        new PortDefinition("A", PortDirection.Input, "float", 0f),
                new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "constant.float",
                    title: "Float",
                    category: "Constants",
                    inputs: [],
                    outputs:
                    [
                        new PortDefinition("Value", PortDirection.Output, "float")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "constant.integer",
                    title: "Integer",
                    category: "Constants",
                    inputs: [],
                    outputs:
                    [
                        new PortDefinition("Value", PortDirection.Output, "int")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "debug.print",
                    title: "Print",
                    category: "Debug",
                    inputs:
                    [
                        new PortDefinition("Value", PortDirection.Input, "object")
                    ],
                    outputs: []));

            _catalog.Register(
                new NodeDefinition(
                    id: "test.typeTest",
                    title: "Testing DataTypes",
                    category: "Type Test",
                    inputs:
                    [
                        new PortDefinition("Target", PortDirection.Input, "none"),
                        new PortDefinition("Yes/No?", PortDirection.Input, "bool", true),
                        new PortDefinition("Name", PortDirection.Input, "string", "Whats your name?"),
                        new PortDefinition("Age", PortDirection.Input, "int", "16"),
                        new PortDefinition("Height", PortDirection.Input, "float", "5.6"),
                        new PortDefinition("Operation", PortDirection.Input, "enum", "Add", new[] { "Add", "Subtract", "Multiply", "Divide" })
                    ],
                    outputs:
                    [
                        new PortDefinition("Output", PortDirection.Output, "string")
                    ]));
        }

        private void AddDemoNodes()
        {
            var addNode = _factory.Create(
                _catalog.Get("math.add"),
                new Point(100, 100));

            var floatCNode = _factory.Create(
                _catalog.Get("constant.float"),
                new Point(100, 300));

            var printNode = _factory.Create(
                _catalog.Get("debug.print"),
                new Point(450, 250));

            NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, addNode));
            NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, floatCNode));
            NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, printNode));
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (Keyboard.Modifiers == ModifierKeys.Control &&
                e.Key == Key.Z)
            {
                NodeCanvas.CommandManager.Undo();
                e.Handled = true;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control &&
                e.Key == Key.Y)
            {
                NodeCanvas.CommandManager.Redo();
                e.Handled = true;
                return;
            }
        }

        private void OnCanvasRightClick(object sender, MouseButtonEventArgs e)
        {
            Point screenPosition = e.GetPosition(NodeCanvas);

            Point canvasPosition =
                NodeCanvas.ScreenToCanvas(screenPosition);

            var menu = NodeMenuBuilder.Build(
                _catalog,
                _factory,
                NodeCanvas.CommandManager,
                NodeCanvas.Graph,
                canvasPosition);

            menu.IsOpen = true;

            e.Handled = true;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            NodeCanvas.Graph.Nodes.Clear();
            NodeCanvas.Graph.Connections.Clear();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}


