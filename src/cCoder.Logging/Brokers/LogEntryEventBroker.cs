// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Logging.Brokers;

internal interface ILogEntryEventBroker
{
    ValueTask RaiseLogEntryAddEventAsync(EventMessage<LogEntry> message);
    ValueTask RaiseLogEntryUpdateEventAsync(EventMessage<LogEntry> message);
    ValueTask RaiseLogEntryDeleteEventAsync(EventMessage<LogEntry> message);
}

internal sealed class LogEntryEventBroker(IEventHub eventHub) : ILogEntryEventBroker
{
    public ValueTask RaiseLogEntryAddEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync(name: "log_entry_add", message: message);

    public ValueTask RaiseLogEntryUpdateEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync(name: "log_entry_update", message: message);

    public ValueTask RaiseLogEntryDeleteEventAsync(EventMessage<LogEntry> message) =>
        eventHub.RaiseEventAsync(name: "log_entry_delete", message: message);
}
