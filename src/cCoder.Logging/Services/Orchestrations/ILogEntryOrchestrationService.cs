// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Orchestrations;

internal interface ILogEntryOrchestrationService
{
    LogEntry GetLogEntry(int logEntryId);
    IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false);
    ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry);
    ValueTask DeleteLogEntryAsync(int logEntryId);
    ValueTask<IEnumerable<OperationResult<LogEntry>>> AddOrUpdateLogEntryResultsAsync(
        IEnumerable<LogEntry> logEntries);
    ValueTask DeleteAllLogEntryAsync(
        IEnumerable<LogEntry> deletedLogEntries);
    ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff);
    int? ResolveAppId(string domainOrName);
}