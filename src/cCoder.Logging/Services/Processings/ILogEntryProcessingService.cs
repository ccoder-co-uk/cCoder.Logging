// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Processings;

public interface ILogEntryProcessingService
{
    LogEntry Get(int id);

    IQueryable<LogEntry> GetAll(bool ignoreFilters = false);

    ValueTask<LogEntry> AddAsync(LogEntry logEntry);

    ValueTask<LogEntry> AddSystemAsync(LogEntry logEntry);

    ValueTask<LogEntry> UpdateAsync(LogEntry logEntry);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<LogEntry>>> AddOrUpdate(IEnumerable<LogEntry> items);

    ValueTask DeleteAllAsync(IEnumerable<LogEntry> items);

    ValueTask<int> DeleteEntriesBeforeAsync(DateTime cutoff);

    int? ResolveAppId(string domainOrName);
}