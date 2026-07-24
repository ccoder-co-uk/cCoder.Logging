// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

internal interface ILogEntryEventService
{
    ValueTask RaiseLogEntryAddEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity);
}
