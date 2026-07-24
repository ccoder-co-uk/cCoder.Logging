// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Foundations;

internal interface ILogEntryService
{
    LogEntry GetLogEntry(int logEntryId);
    IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false);
    ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry);
    ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry);
    ValueTask DeleteLogEntryAsync(int logEntryId);
    ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff);
    int? ResolveAppId(string domainOrName);
}
