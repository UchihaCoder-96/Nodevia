namespace Nodevia.Commands;

public interface INodeCommand
{
    string Name { get; }

    void Execute();

    void Undo();
}

