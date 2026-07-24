// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal class LogEntryOrchestrationService(ILogEntryProcessingService processingService, ILogEntryEventProcessingService eventService) : ILogEntryOrchestrationService
{
    public LogEntry Get(int id)
    {
        return processingService.Get(id);
    }

    public IQueryable<LogEntry> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<LogEntry> AddAsync(LogEntry logEntry)
    {
        LogEntry result = await processingService.AddAsync(logEntry);
        await eventService.RaiseLogEntryAddEventAsync(result);
        return result;
    }

    public async ValueTask<LogEntry> AddSystemAsync(LogEntry logEntry)
    {
        LogEntry result = await processingService.AddSystemAsync(logEntry);
        await eventService.RaiseLogEntryAddEventAsync(result);
        return result;
    }

    public async ValueTask<LogEntry> UpdateAsync(LogEntry logEntry)
    {
        LogEntry result = await processingService.UpdateAsync(logEntry);
        await eventService.RaiseLogEntryUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        LogEntry entity = processingService.Get(id);
        await eventService.RaiseLogEntryDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<LogEntry>>> AddOrUpdate(IEnumerable<LogEntry> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<LogEntry> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    public ValueTask<int> DeleteEntriesBeforeAsync(DateTime cutoff)
    {
        return processingService.DeleteEntriesBeforeAsync(cutoff);
    }

    public int? ResolveAppId(string domainOrName)
    {
        return processingService.ResolveAppId(domainOrName);
    }
}