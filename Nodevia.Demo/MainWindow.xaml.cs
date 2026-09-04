using Nodevia.Controls;
using Nodevia.Models;
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

            NodeCanvas.Nodes.Add(
                new Node
                {
                    Title = "Multiply",
                    Position = new Point(450, 250)
                });

            var addNode = new Node { Title = "Add", Position = new Point(100, 100) };
            addNode.InputPorts.Add(new Port("A", PortDirection.Input, "float"));
            addNode.InputPorts.Add(new Port("B", PortDirection.Input, "float"));
            addNode.OutputPorts.Add(new Port("Result", PortDirection.Output, "float"));

            // addNode.InputPorts.Add(new Port("Bad", PortDirection.Output)); // not possible, hehe...

            NodeCanvas.Nodes.Add(addNode);

        }
    }
}


