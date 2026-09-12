using Nodevia.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Nodevia.Controls;

public class ToastHost : Control
{
    static ToastHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ToastHost),
            new FrameworkPropertyMetadata(typeof(ToastHost)));
    }

    public static readonly DependencyProperty LogProperty =
        DependencyProperty.Register(nameof(Log), typeof(GraphLog), typeof(ToastHost),
            new FrameworkPropertyMetadata(null, OnLogChanged));

    public GraphLog? Log
    {
        get => (GraphLog?)GetValue(LogProperty);
        set => SetValue(LogProperty, value);
    }

    public ObservableCollection<ToastItem> Toasts { get; } = new();

    private static void OnLogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (ToastHost)d;

        if (e.OldValue is GraphLog oldLog)
            oldLog.Entries.CollectionChanged -= host.OnEntriesChanged;

        if (e.NewValue is GraphLog newLog)
            newLog.Entries.CollectionChanged += host.OnEntriesChanged;
    }

    private void OnEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null)
            return;

        foreach (LogEntry entry in e.NewItems)
        {
            if (entry.Severity is LogSeverity.Warning or LogSeverity.Error)
                Toasts.Add(new ToastItem(entry, Dismiss));
        }
    }

    private void Dismiss(ToastItem toast) => Toasts.Remove(toast);
}

