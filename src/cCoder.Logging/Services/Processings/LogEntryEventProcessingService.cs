// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal class LogEntryEventProcessingService(ILogEntryEventService eventService) : ILogEntryEventProcessingService
{
    public ValueTask RaiseLogEntryAddEventAsync(LogEntry entity) => eventService.RaiseLogEntryAddEventAsync(entity);

    public ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity) => eventService.RaiseLogEntryUpdateEventAsync(entity);

    public ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity) => eventService.RaiseLogEntryDeleteEventAsync(entity);
}