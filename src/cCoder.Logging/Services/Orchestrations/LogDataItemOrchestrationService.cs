// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal class LogDataItemOrchestrationService(ILogDataItemProcessingService processingService, ILogDataItemEventProcessingService eventService) : ILogDataItemOrchestrationService
{
    public LogDataItem Get(int id)
    {
        return processingService.Get(id);
    }

    public IQueryable<LogDataItem> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<LogDataItem> AddAsync(LogDataItem logDataItem)
    {
        LogDataItem result = await processingService.AddAsync(logDataItem);
        await eventService.RaiseLogDataItemAddEventAsync(result);
        return result;
    }

    public async ValueTask<LogDataItem> UpdateAsync(LogDataItem logDataItem)
    {
        LogDataItem result = await processingService.UpdateAsync(logDataItem);
        await eventService.RaiseLogDataItemUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        LogDataItem entity = processingService.Get(id);
        await eventService.RaiseLogDataItemDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<LogDataItem>>> AddOrUpdate(IEnumerable<LogDataItem> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<LogDataItem> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}