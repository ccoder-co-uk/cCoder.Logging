// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Microsoft.EntityFrameworkCore;
using DataLogEntry = cCoder.Data.Models.Logging.LogEntry;


namespace cCoder.Logging.Brokers;

internal interface ILogEntryBroker
{
    IQueryable<DataLogEntry> SelectAllLogEntries();
    IQueryable<DataLogEntry> SelectAllLogEntriesIgnoringFilters();
    ValueTask<DataLogEntry> InsertLogEntryAsync(DataLogEntry newLogEntry);
    ValueTask<DataLogEntry> UpdateLogEntryAsync(DataLogEntry updatedLogEntry);
    ValueTask<int> DeleteLogEntryAsync(DataLogEntry deletedLogEntry);
    ValueTask DeleteAllLogEntriesAsync(IEnumerable<DataLogEntry> deletedLogEntries);
    ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff);
    int? SelectAppIdByDomainOrName(string domainOrName);
    string SelectTenantIdByAppId(int appId);
}

internal sealed class LogEntryBroker(
    ICoreContextFactory coreContextFactory)
        : ILogEntryBroker
{
    public IQueryable<DataLogEntry> SelectAllLogEntries()
    {
        CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Logs;
    }

    public IQueryable<DataLogEntry> SelectAllLogEntriesIgnoringFilters()
    {
        CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Logs.IgnoreQueryFilters();
    }

    public async ValueTask<DataLogEntry> InsertLogEntryAsync(
        DataLogEntry newLogEntry)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        DataLogEntry result = (
            await coreDataContext.Logs.AddAsync(entity: newLogEntry)).Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<DataLogEntry> UpdateLogEntryAsync(
        DataLogEntry updatedLogEntry)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        DataLogEntry result =
            coreDataContext.Logs.Update(entity: updatedLogEntry).Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLogEntryAsync(
        DataLogEntry deletedLogEntry)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        coreDataContext.Logs.Remove(entity: deletedLogEntry);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLogEntriesAsync(
        IEnumerable<DataLogEntry> deletedLogEntries)
    {
        DataLogEntry[] logEntries = [.. deletedLogEntries];

        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        coreDataContext.Logs.RemoveRange(entities: logEntries);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        DataLogEntry[] items = await coreDataContext.Logs
            .IgnoreQueryFilters()
            .Where(predicate: logEntry => logEntry.Date < cutoff)
            .ToArrayAsync();

        coreDataContext.Logs.RemoveRange(entities: items);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppIdByDomainOrName(string domainOrName)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .IgnoreQueryFilters()
            .Where(predicate: app =>
                app.Name == domainOrName
                || app.Domain == domainOrName)
            .Select(selector: app => (int?)app.Id)
            .FirstOrDefault();
    }

    public string SelectTenantIdByAppId(int appId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .IgnoreQueryFilters()
            .Where(predicate: app => app.Id == appId)
            .Select(selector: app => app.TenantId)
            .FirstOrDefault();
    }
}