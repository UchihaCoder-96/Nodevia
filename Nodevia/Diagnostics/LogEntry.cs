namespace Nodevia.Diagnostics;

public enum LogSeverity { Info, Warning, Error }

public class LogEntry
{
    public LogSeverity Severity { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }

    public LogEntry(LogSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
        Timestamp = DateTime.Now;
    }
}

