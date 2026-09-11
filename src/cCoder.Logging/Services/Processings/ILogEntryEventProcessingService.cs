// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryEventProcessingService
{
    ValueTask RaiseLogEntryAddEventAsync(LogEntry logEntry);
    ValueTask RaiseLogEntryUpdateEventAsync(LogEntry logEntry);
    ValueTask RaiseLogEntryDeleteEventAsync(LogEntry logEntry);
}