// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryEventProcessingService
{
    ValueTask RaiseLogEntryAddEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity);
}