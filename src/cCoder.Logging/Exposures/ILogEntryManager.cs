// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Exposures;

public interface ILogEntryManager
{
    LogEntry GetLogEntry(int logEntryId);
    IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false);
    ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry);
    ValueTask DeleteLogEntryAsync(int logEntryId);
    ValueTask<IEnumerable<Result<LogEntry>>> AddOrUpdateLogEntriesAsync(
        IEnumerable<LogEntry> logEntries);
    ValueTask DeleteAllLogEntriesAsync(
        IEnumerable<LogEntry> deletedLogEntries);
    ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff);
    int? ResolveAppId(string domainOrName);
}