using Nodevia.Execution;
using Nodevia.Commands;
using Nodevia.Controls;
using Nodevia.Models;
using Nodevia.Nodes;
using Nodevia.UI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nodevia.Demo
{
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
            NodeCanvas.CommandManager.StateChanged += (_, _) => RefreshPrintNodes();
            NodeCanvas.Graph.ValueChanged += (_, _) => RefreshPrintNodes();

            RefreshPrintNodes();
        }

        private void RefreshPrintNodes()
        {
            var evaluator = new GraphEvaluator(NodeCanvas.Graph);

            foreach (var node in NodeCanvas.Graph.Nodes)
            {
                if (node.Behavior is not PrintBehavior)
                    continue;

                try
                {
                    object? value = evaluator.GetInputValue(node, "Value");
                    node.Subtitle = value?.ToString() ?? "null";
                }
                catch (Exception ex)
                {
                    node.Subtitle = $"[WARNING] : {ex.Message}";
                }
            }
        }

        private void RegisterNodes()
        {
            _catalog.Register(
                new NodeDefinition(
                    id: "math.add",
                    title: "Add",
                    category: "Math",
                    inputs: [
                        new PortDefinition("A", PortDirection.Input, "float", 0f), 
                        new PortDefinition("B", PortDirection.Input, "float", 0f)
                    ],
                    outputs: [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ],
                    behavior: new AddBehavior()));

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
                    ],
                    behavior: new SubtractBehavior()));

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
                    ],
                    behavior: new MultiplyBehavior()));

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
                    ],
                    behavior: new DivideBehavior()));

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
                    inputs: [
                        new PortDefinition("C", PortDirection.Input, "float", 0f, allowConnections: false)
                    ],
                    outputs: [
                        new PortDefinition("Result", PortDirection.Output, "float")
                    ],
                    behavior: new ConstantFBehavior()));

            _catalog.Register(
                new NodeDefinition(
                    id: "constant.integer",
                    title: "Integer",
                    category: "Constants",
                    inputs: [
                        new PortDefinition("C", PortDirection.Input, "int", 0)
                    ],
                    outputs:
                    [
                        new PortDefinition("Result", PortDirection.Output, "int")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "debug.print",
                    title: "Print",
                    category: "Debug",
                    inputs: [new PortDefinition("Value", PortDirection.Input, "none")],
                    outputs: [],
                    behavior: new PrintBehavior(),
                    bodyTemplateKey: "debug-console"));

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
                        new PortDefinition("Age", PortDirection.Input, "int", defaultValue: 16, metadata: NumericPortOptions.Create(min: 0, max: 120, step: 1, isInteger: true)),
                        new PortDefinition("Height", PortDirection.Input, "float", defaultValue: 5.6, metadata: NumericPortOptions.Create(min: 0, max: 10, step: 0.1)),
                        new PortDefinition("Operation", PortDirection.Input, "enum", "Add", new[] { "Add", "Subtract", "Multiply", "Divide" }),
                        new PortDefinition("Volume", PortDirection.Input, "slider", defaultValue: 0, metadata: SliderPortOptions.Create(min: -5, max: 5, step: 1)),
                        new PortDefinition("Size", PortDirection.Input, "vec2", defaultValue: new Vec2(0, 0)),
                        new PortDefinition("Position", PortDirection.Input, "vec3", defaultValue: new Vec3(0, 0, 0)),
                        new PortDefinition("Color", PortDirection.Input, "color", defaultValue: Colors.Red)
                    ],
                    outputs:
                    [
                        new PortDefinition("Output", PortDirection.Output, "string")
                    ]));

            _catalog.Register(
                new NodeDefinition(
                    id: "image.posterize",
                    title: "Posterize",
                    category: "Image",
                    inputs: [
                        new PortDefinition("Image", PortDirection.Input, "image"),
                        new PortDefinition("Threshold", PortDirection.Input, "float", 0.5f)
                    ],
                    outputs: [new PortDefinition("Result", PortDirection.Output, "image")],
                    behavior: new PosterizeBehavior(),
                    bodyTemplateKey: "posterize-preview"));

            _catalog.Register(
                new NodeDefinition(
                    id: "image.img_input",
                    title: "Input Image",
                    category: "Image",
                    inputs: [
                        new PortDefinition("Path", PortDirection.Input, "string", allowConnections: false)
                    ],
                    outputs: [new PortDefinition("Result", PortDirection.Output, "image")],
                    behavior: new ImageInputBehavior()));

        }

        private void AddDemoNodes()
        {
            var addNode = _factory.Create(
                _catalog.Get("image.img_input"),
                new Point(100, 100));

            var floatCNode = _factory.Create(
                _catalog.Get("constant.float"),
                new Point(100, 300));

            var printNode = _factory.Create(
                _catalog.Get("image.posterize"),
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
            RefreshPrintNodes();
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


