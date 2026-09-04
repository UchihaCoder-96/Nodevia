namespace Nodevia.Commands;

public class CommandManager
{
    private readonly Stack<INodeCommand> _undoStack = new();
    private readonly Stack<INodeCommand> _redoStack = new();

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public event EventHandler? StateChanged;

    public void Execute(INodeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        command.Execute();

        _undoStack.Push(command);
        _redoStack.Clear();

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;

        var command = _undoStack.Pop();

        command.Undo();
        _redoStack.Push(command);

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            return;

        var command = _redoStack.Pop();

        command.Execute();
        _undoStack.Push(command);

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();

        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}

