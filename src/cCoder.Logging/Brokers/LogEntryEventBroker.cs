using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.Logging.Brokers;

public interface ILogEntryEventBroker
{
    ValueTask RaiseLogEntryAddEventAsync(EventMessage<LogEntry> message);
    ValueTask RaiseLogEntryUpdateEventAsync(EventMessage<LogEntry> message);
    ValueTask RaiseLogEntryDeleteEventAsync(EventMessage<LogEntry> message);
}

internal class LogEntryEventBroker(IEventHub eventHub) : ILogEntryEventBroker
{
    public ValueTask RaiseLogEntryAddEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync("log_entry_add", message);

    public ValueTask RaiseLogEntryUpdateEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync("log_entry_update", message);

    public ValueTask RaiseLogEntryDeleteEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync("log_entry_delete", message);
}



