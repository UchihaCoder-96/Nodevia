using System.Collections.ObjectModel;

namespace Nodevia.Diagnostics;

public class GraphLog
{
    private const int MaxEntries = 500;

    public ObservableCollection<LogEntry> Entries { get; } = new();

    public void Info(string message) => Add(LogSeverity.Info, message);
    public void Warning(string message) => Add(LogSeverity.Warning, message);
    public void Error(string message) => Add(LogSeverity.Error, message);

    private void Add(LogSeverity severity, string message)
    {
        Entries.Add(new LogEntry(severity, message));

        while (Entries.Count > MaxEntries)
            Entries.RemoveAt(0);
    }

    public void Clear() => Entries.Clear();
}

