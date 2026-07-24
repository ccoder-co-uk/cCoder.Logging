// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Microsoft.EntityFrameworkCore;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;


namespace cCoder.Logging.Brokers;

internal interface ILogDataItemBroker
{
    IQueryable<DataLogDataItem> SelectAllLogDataItems();
    IQueryable<DataLogDataItem> SelectAllLogDataItemsIgnoringFilters();
    ValueTask<DataLogDataItem> InsertLogDataItemAsync(DataLogDataItem newLogDataItem);
    ValueTask<DataLogDataItem> UpdateLogDataItemAsync(DataLogDataItem updatedLogDataItem);
    ValueTask<int> DeleteLogDataItemAsync(DataLogDataItem deletedLogDataItem);
    ValueTask DeleteAllLogDataItemsAsync(IEnumerable<DataLogDataItem> deletedLogDataItems);
    int? SelectAppIdByLogDataItem(DataLogDataItem logDataItem);
}

internal sealed class LogDataItemBroker(
    ICoreContextFactory coreContextFactory)
        : ILogDataItemBroker
{
    public IQueryable<DataLogDataItem> SelectAllLogDataItems()
    {
        CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.LogData;
    }

    public IQueryable<DataLogDataItem> SelectAllLogDataItemsIgnoringFilters()
    {
        CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.LogData.IgnoreQueryFilters();
    }

    public async ValueTask<DataLogDataItem> InsertLogDataItemAsync(
        DataLogDataItem newLogDataItem)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        DataLogDataItem result = (
            await coreDataContext.LogData.AddAsync(entity: newLogDataItem)).Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<DataLogDataItem> UpdateLogDataItemAsync(
        DataLogDataItem updatedLogDataItem)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        DataLogDataItem result =
            coreDataContext.LogData.Update(entity: updatedLogDataItem).Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLogDataItemAsync(
        DataLogDataItem deletedLogDataItem)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        coreDataContext.LogData.Remove(entity: deletedLogDataItem);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLogDataItemsAsync(
        IEnumerable<DataLogDataItem> deletedLogDataItems)
    {
        DataLogDataItem[] logDataItems = [.. deletedLogDataItems];

        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        coreDataContext.LogData.RemoveRange(entities: logDataItems);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppIdByLogDataItem(DataLogDataItem logDataItem)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Logs
            .Where(predicate: log => log.Id == logDataItem.LogEntryId)
            .Join(
                inner: coreDataContext.Apps,
                outerKeySelector: log => log.AppName,
                innerKeySelector: app => app.Name,
                resultSelector: (log, app) => (int?)app.Id)
            .FirstOrDefault();
    }
}