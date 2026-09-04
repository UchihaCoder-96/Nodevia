using Nodevia.Controls;
using Nodevia.Models;
using Nodevia.Rendering;
using Nodevia.Routing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Nodevia.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddDemoNodes();
        }

        private void AddDemoNodes()
        {
            var addNode = CreateBinaryNode(
                "Add",
                new Point(100, 100));

            var subtractNode = CreateBinaryNode(
                "Subtract",
                new Point(100, 300));

            var printNode = CreateUnaryNode(
                "Print",
                new Point(450, 200));

            NodeCanvas.Graph.Nodes.Add(addNode);
            NodeCanvas.Graph.Nodes.Add(subtractNode);
            NodeCanvas.Graph.Nodes.Add(printNode);
        }

        private static Node CreateBinaryNode(string title,Point position)
        {
            var node = new Node
            {
                Title = title,
                Position = position
            };

            node.InputPorts.Add(
                new Port("A", PortDirection.Input, "float"));

            node.InputPorts.Add(
                new Port("B", PortDirection.Input, "float"));

            node.OutputPorts.Add(
                new Port("Result", PortDirection.Output, "float"));

            return node;
        }

        private static Node CreateUnaryNode(string title,Point position)
        {
            var node = new Node
            {
                Title = title,
                Position = position
            };

            node.InputPorts.Add(
                new Port("Value", PortDirection.Input, "float"));

            node.OutputPorts.Add(
                new Port("Result", PortDirection.Output, "float"));

            return node;
        }

        private static Node CreateConstantNode(Point position)
        {
            var node = new Node
            {
                Title = "Constant",
                Position = position
            };

            node.OutputPorts.Add(new Port("Value", PortDirection.Output, "float"));

            return node;
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateBinaryNode("Add",GetNewNodePosition()));
        }


        private void SubtractNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateBinaryNode("Subtract",GetNewNodePosition()));
        }


        private void MultiplyNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(
                CreateBinaryNode("Multiply",GetNewNodePosition()));
        }

        private void DivideNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateBinaryNode("Divide", GetNewNodePosition()));
        }

        private void ConstantNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateConstantNode(GetNewNodePosition()));
        }

        private void NegateNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateUnaryNode("Negate",GetNewNodePosition()));
        }

        private void PrintNode_Click(object sender, RoutedEventArgs e)
        {
            AddNode(CreateUnaryNode("Print",GetNewNodePosition()));
        }

        private void AddNode(Node node)
        {
            NodeCanvas.Graph.Nodes.Add(node);
        }

        private Point GetNewNodePosition()
        {
            return new Point(200, 150);
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


