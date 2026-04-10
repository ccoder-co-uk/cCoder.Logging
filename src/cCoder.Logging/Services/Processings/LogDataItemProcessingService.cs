using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Processings;

internal class LogDataItemProcessingService(ILogDataItemService service) : ILogDataItemProcessingService
{
    public LogDataItem Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<LogDataItem> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<LogDataItem> AddAsync(LogDataItem logDataItem)
    {
        return service.AddAsync(logDataItem);
    }

    public ValueTask<LogDataItem> UpdateAsync(LogDataItem logDataItem)
    {
        return service.UpdateAsync(logDataItem);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<LogDataItem>>> AddOrUpdate(IEnumerable<LogDataItem> items)
    {
        return service.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<LogDataItem> items)
    {
        return service.DeleteAllAsync(items);
    }
}
