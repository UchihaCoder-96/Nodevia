using Nodevia.Models;
using System.Windows;

namespace Nodevia.Commands;

public class MoveNodeCommand : INodeCommand
{
    private readonly Node _node;
    private readonly Point _from;
    private readonly Point _to;

    public string Name => "Move Node";

    public MoveNodeCommand(Node node, Point from, Point to)
    {
        _node = node;
        _from = from;
        _to = to;
    }

    public void Execute() => _node.Position = _to;

    public void Undo() => _node.Position = _from;
}

