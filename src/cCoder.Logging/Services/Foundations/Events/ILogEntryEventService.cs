// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

internal interface ILogEntryEventService
{
    ValueTask RaiseLogEntryAddEventAsync(LogEntry logEntry);
    ValueTask RaiseLogEntryUpdateEventAsync(LogEntry logEntry);
    ValueTask RaiseLogEntryDeleteEventAsync(LogEntry logEntry);
}