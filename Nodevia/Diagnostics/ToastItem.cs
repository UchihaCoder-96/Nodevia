using System.Windows.Input;

namespace Nodevia.Diagnostics;

public class ToastItem
{
    public LogEntry Entry { get; }
    public ICommand DismissCommand { get; }

    public ToastItem(LogEntry entry, Action<ToastItem> onDismiss)
    {
        Entry = entry;
        DismissCommand = new RelayCommand(() => onDismiss(this));
    }
}


internal class RelayCommand : ICommand
{
    private readonly Action _execute;

    public RelayCommand(Action execute) => _execute = execute;

    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
}

