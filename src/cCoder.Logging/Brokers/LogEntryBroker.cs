using cCoder.Data;
using Microsoft.EntityFrameworkCore;
using DataLogEntry = cCoder.Data.Models.Logging.LogEntry;


namespace cCoder.Logging.Brokers;

public interface ILogEntryBroker
{
    IQueryable<DataLogEntry> GetAllLogEntries(bool ignoreFilters);
    ValueTask<DataLogEntry> AddLogEntryAsync(DataLogEntry entity);
    ValueTask<DataLogEntry> UpdateLogEntryAsync(DataLogEntry entity);
    ValueTask<int> DeleteLogEntryAsync(DataLogEntry entity);
    ValueTask DeleteAllLogEntriesAsync(IEnumerable<DataLogEntry> items);
    int? GetAppId(DataLogEntry entity);
}

internal class LogEntryBroker(ICoreContextFactory coreContextFactory) : ILogEntryBroker
{
    public IQueryable<DataLogEntry> GetAllLogEntries(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.Logs.IgnoreQueryFilters()
            : coreDataContext.Logs;
    }

    public async ValueTask<DataLogEntry> AddLogEntryAsync(DataLogEntry entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        DataLogEntry result = (await coreDataContext.Logs.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<DataLogEntry> UpdateLogEntryAsync(DataLogEntry entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        DataLogEntry result = coreDataContext.Logs.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLogEntryAsync(DataLogEntry entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Logs.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLogEntriesAsync(IEnumerable<DataLogEntry> items)
    {
        DataLogEntry[] itemArray = [.. items];
        if (itemArray.Length == 0)
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Logs.RemoveRange(itemArray);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(DataLogEntry entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Apps
            .Where(app => app.Name == entity.AppName)
            .Select(app => (int?)app.Id)
            .FirstOrDefault();
    }
}




