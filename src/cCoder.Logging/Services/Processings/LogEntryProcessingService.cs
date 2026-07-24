// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Processings;

internal class LogEntryProcessingService(ILogEntryService service) : ILogEntryProcessingService
{
    public LogEntry Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<LogEntry> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<LogEntry> AddAsync(LogEntry logEntry)
    {
        return service.AddAsync(logEntry);
    }

    public ValueTask<LogEntry> AddSystemAsync(LogEntry logEntry)
    {
        return service.AddSystemAsync(logEntry);
    }

    public ValueTask<LogEntry> UpdateAsync(LogEntry logEntry)
    {
        return service.UpdateAsync(logEntry);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<LogEntry>>> AddOrUpdate(IEnumerable<LogEntry> items)
    {
        return service.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<LogEntry> items)
    {
        return service.DeleteAllAsync(items);
    }

    public ValueTask<int> DeleteEntriesBeforeAsync(DateTime cutoff)
    {
        return service.DeleteEntriesBeforeAsync(cutoff);
    }

    public int? ResolveAppId(string domainOrName)
    {
        return service.ResolveAppId(domainOrName);
    }
}