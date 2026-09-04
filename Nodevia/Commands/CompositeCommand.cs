namespace Nodevia.Commands;

public class CompositeCommand : INodeCommand
{
    private readonly List<INodeCommand> _commands = new();

    public string Name { get; }

    public CompositeCommand(string name)
    {
        Name = name;
    }

    public bool HasCommands => _commands.Count > 0;

    public void Add(INodeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commands.Add(command);
    }

    public void Execute()
    {
        foreach (var command in _commands)
            command.Execute();
    }

    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
            _commands[i].Undo();
    }
}

