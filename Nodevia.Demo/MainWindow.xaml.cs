using Microsoft.Win32;
using Nodevia.Commands;
using Nodevia.Models;
using Nodevia.Nodes;
using Nodevia.Serialization;
using Nodevia.UI;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Nodevia.Demo;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly NodeCatalog _catalog = new();
    private readonly NodeFactory _factory = new();
    private readonly JsonGraphSerializer _serializer = new();
    private readonly DispatcherTimer _statusTimer;

    private string? _currentFilePath;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private bool _isStatusBarVisible = true;
    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set { if (_isStatusBarVisible == value) return; _isStatusBarVisible = value; OnPropertyChanged(); }
    }

    private bool _isConsoleVisible = true;
    public bool IsConsoleVisible
    {
        get => _isConsoleVisible;
        set { if (_isConsoleVisible == value) return; _isConsoleVisible = value; OnPropertyChanged(); }
    }

    public string NodeCountText => $"Nodes: {NodeCanvas.Graph.Nodes.Count}";
    public string ConnectionCountText => $"Connections: {NodeCanvas.Graph.Connections.Count}";
    public string SelectedCountText => $"Selected: {NodeCanvas.Graph.Nodes.Count(n => n.IsSelected)}";
    public string ZoomText => $"Zoom: {NodeCanvas.Zoom:P0}";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        RegisterNodes();
        AddStarterNodes();

        NodeCanvas.MouseRightButtonUp += OnCanvasRightClick;
        NodeCanvas.CommandManager.StateChanged += (_, _) => RefreshStatusBar();
        NodeCanvas.Graph.Nodes.CollectionChanged += (_, _) => RefreshStatusBar();
        NodeCanvas.Graph.Connections.CollectionChanged += (_, _) => RefreshStatusBar();

        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _statusTimer.Tick += (_, _) =>
        {
            OnPropertyChanged(nameof(SelectedCountText));
            OnPropertyChanged(nameof(ZoomText));
        };
        _statusTimer.Start();

        RefreshStatusBar();
    }

    private void RefreshStatusBar()
    {
        OnPropertyChanged(nameof(NodeCountText));
        OnPropertyChanged(nameof(ConnectionCountText));
        OnPropertyChanged(nameof(SelectedCountText));
    }

    private void RegisterNodes()
    {
        _catalog.Register(new NodeDefinition(
            id: "value.number",
            title: "Number",
            category: "Values",
            inputs: [new PortDefinition("Value", PortDirection.Input, "float", 0.0, allowConnections: false)],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new ConstantBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.add",
            title: "Add",
            category: "Math",
            inputs: [
                new PortDefinition("A", PortDirection.Input, "float", 0.0),
                new PortDefinition("B", PortDirection.Input, "float", 0.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new AddBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.subtract",
            title: "Subtract",
            category: "Math",
            inputs: [
                new PortDefinition("A", PortDirection.Input, "float", 0.0),
                new PortDefinition("B", PortDirection.Input, "float", 0.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new SubtractBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.multiply",
            title: "Multiply",
            category: "Math",
            inputs: [
                new PortDefinition("A", PortDirection.Input, "float", 1.0),
                new PortDefinition("B", PortDirection.Input, "float", 1.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new MultiplyBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.divide",
            title: "Divide",
            category: "Math",
            inputs: [
                new PortDefinition("A", PortDirection.Input, "float", 1.0),
                new PortDefinition("B", PortDirection.Input, "float", 1.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new DivideBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.power",
            title: "Power",
            category: "Math",
            inputs: [
                new PortDefinition("Base", PortDirection.Input, "float", 1.0),
                new PortDefinition("Exponent", PortDirection.Input, "float", 2.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new PowerBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.modulo",
            title: "Modulo",
            category: "Math",
            inputs: [
                new PortDefinition("A", PortDirection.Input, "float", 1.0),
                new PortDefinition("B", PortDirection.Input, "float", 1.0)
            ],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new ModuloBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.sqrt",
            title: "Square Root",
            category: "Math",
            inputs: [new PortDefinition("Value", PortDirection.Input, "float", 0.0)],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new SqrtBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "math.negate",
            title: "Negate",
            category: "Math",
            inputs: [new PortDefinition("Value", PortDirection.Input, "float", 0.0)],
            outputs: [new PortDefinition("Result", PortDirection.Output, "float")],
            behavior: new NegateBehavior()));

        _catalog.Register(new NodeDefinition(
            id: "output.result",
            title: "Result",
            category: "Output",
            inputs: [new PortDefinition("Value", PortDirection.Input, "none")],
            outputs: [],
            behavior: new DisplayBehavior(),
            bodyTemplateKey: "display-result"));
    }

    private void AddStarterNodes()
    {
        var a = _factory.Create(_catalog.Get("value.number"), new Point(80, 120));
        var b = _factory.Create(_catalog.Get("value.number"), new Point(80, 260));
        var add = _factory.Create(_catalog.Get("math.add"), new Point(360, 190));
        var result = _factory.Create(_catalog.Get("output.result"), new Point(620, 190));

        NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, a));
        NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, b));
        NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, add));
        NodeCanvas.CommandManager.Execute(new AddNodeCommand(NodeCanvas.Graph, result));
    }

    private void OnCanvasRightClick(object sender, MouseButtonEventArgs e)
    {
        Point screenPosition = e.GetPosition(NodeCanvas);
        Point canvasPosition = NodeCanvas.ScreenToCanvas(screenPosition);

        var menu = NodeMenuBuilder.Build(_catalog, _factory, NodeCanvas.CommandManager, NodeCanvas.Graph, canvasPosition);
        menu.IsOpen = true;

        e.Handled = true;
    }

    private void New_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Discard the current graph and start a new one?", "New Graph",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        NodeCanvas.Graph = new NodeGraph();
        NodeCanvas.CommandManager.Clear();
        _currentFilePath = null;
        RefreshStatusBar();
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Nodevia Graph (*.nodevia)|*.nodevia|All files|*.*" };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            string json = File.ReadAllText(dialog.FileName);
            NodeCanvas.Graph = _serializer.Deserialize(json, _catalog, _factory);
            NodeCanvas.CommandManager.Clear();
            _currentFilePath = dialog.FileName;
            RefreshStatusBar();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open file:\n{ex.Message}", "Open Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_currentFilePath is null)
        {
            SaveAs_Click(sender, e);
            return;
        }

        SaveToFile(_currentFilePath);
    }

    private void SaveAs_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "Nodevia Graph (*.nodevia)|*.nodevia|All files|*.*" };

        if (dialog.ShowDialog() != true)
            return;

        _currentFilePath = dialog.FileName;
        SaveToFile(_currentFilePath);
    }

    private void SaveToFile(string path)
    {
        try
        {
            string json = _serializer.Serialize(NodeCanvas.Graph);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save file:\n{ex.Message}", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void Undo_Click(object sender, RoutedEventArgs e) => NodeCanvas.CommandManager.Undo();
    private void Redo_Click(object sender, RoutedEventArgs e) => NodeCanvas.CommandManager.Redo();

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (Keyboard.Modifiers != ModifierKeys.Control)
            return;

        switch (e.Key)
        {
            case Key.Z: NodeCanvas.CommandManager.Undo(); e.Handled = true; break;
            case Key.Y: NodeCanvas.CommandManager.Redo(); e.Handled = true; break;
            case Key.N: New_Click(this, new RoutedEventArgs()); e.Handled = true; break;
            case Key.O: Open_Click(this, new RoutedEventArgs()); e.Handled = true; break;
            case Key.S: Save_Click(this, new RoutedEventArgs()); e.Handled = true; break;
        }
    }
}

