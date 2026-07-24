// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Microsoft.EntityFrameworkCore;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;


namespace cCoder.Logging.Brokers;

public interface ILogDataItemBroker
{
    IQueryable<DataLogDataItem> GetAllLogDataItems(bool ignoreFilters);
    ValueTask<DataLogDataItem> AddLogDataItemAsync(DataLogDataItem entity);
    ValueTask<DataLogDataItem> UpdateLogDataItemAsync(DataLogDataItem entity);
    ValueTask<int> DeleteLogDataItemAsync(DataLogDataItem entity);
    ValueTask DeleteAllLogDataItemsAsync(IEnumerable<DataLogDataItem> items);
    int? GetAppId(DataLogDataItem entity);
}

internal class LogDataItemBroker(ICoreContextFactory coreContextFactory) : ILogDataItemBroker
{
    public IQueryable<DataLogDataItem> GetAllLogDataItems(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.LogData.IgnoreQueryFilters()
            : coreDataContext.LogData;
    }

    public async ValueTask<DataLogDataItem> AddLogDataItemAsync(DataLogDataItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        DataLogDataItem result = (await coreDataContext.LogData.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<DataLogDataItem> UpdateLogDataItemAsync(DataLogDataItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        DataLogDataItem result = coreDataContext.LogData.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLogDataItemAsync(DataLogDataItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.LogData.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLogDataItemsAsync(IEnumerable<DataLogDataItem> items)
    {
        DataLogDataItem[] itemArray = [.. items];
        if (itemArray.Length == 0)
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.LogData.RemoveRange(itemArray);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(DataLogDataItem entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Logs
            .Where(log => log.Id == entity.LogEntryId)
            .Join(
                coreDataContext.Apps,
                log => log.AppName,
                app => app.Name,
                (log, app) => (int?)app.Id)
            .FirstOrDefault();
    }
}