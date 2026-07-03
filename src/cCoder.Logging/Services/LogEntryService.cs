using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services;

internal class LogEntryService(ILogEntryBroker logEntryBroker, IAuthorizationBroker authorizationBroker) : ILogEntryService
{
    public cCoder.Data.Models.Logging.LogEntry Get(int id)
    {
        return (from item in logEntryBroker.GetAllLogEntries(ignoreFilters: false)
                where item.Id == id
                select new cCoder.Data.Models.Logging.LogEntry
                {
                    Id = item.Id,
                    AppId = item.AppId,
                    Level = item.Level,
                    Message = item.Message,
                    AppName = item.AppName,
                    TypeName = item.TypeName,
                    Date = item.Date,
                    Data = item.Data.Select((cCoder.Data.Models.Logging.LogDataItem dataItem) => new cCoder.Data.Models.Logging.LogDataItem
                    {
                        Id = dataItem.Id,
                        LogEntryId = dataItem.LogEntryId,
                        Name = dataItem.Name,
                        Value = dataItem.Value
                    })
                }).FirstOrDefault();
    }

    public IQueryable<cCoder.Data.Models.Logging.LogEntry> GetAll(bool ignoreFilters = false)
    {
        return logEntryBroker.GetAllLogEntries(ignoreFilters);
    }

    public async ValueTask<cCoder.Data.Models.Logging.LogEntry> AddAsync(cCoder.Data.Models.Logging.LogEntry logEntry)
    {
        cCoder.Data.Models.Logging.LogEntry internalLogEntry = ToInternalLogEntry(logEntry);
        authorizationBroker.Authorize(logEntryBroker.GetAppId(internalLogEntry), "LogEntry_create");
        return ToExternalLogEntry(await logEntryBroker.AddLogEntryAsync(internalLogEntry));
    }

    public async ValueTask<cCoder.Data.Models.Logging.LogEntry> AddSystemAsync(cCoder.Data.Models.Logging.LogEntry logEntry)
    {
        cCoder.Data.Models.Logging.LogEntry internalLogEntry = ToInternalLogEntry(logEntry);
        return ToExternalLogEntry(await logEntryBroker.AddLogEntryAsync(internalLogEntry));
    }

    public async ValueTask<cCoder.Data.Models.Logging.LogEntry> UpdateAsync(cCoder.Data.Models.Logging.LogEntry logEntry)
    {
        cCoder.Data.Models.Logging.LogEntry internalLogEntry = ToInternalLogEntry(logEntry);
        authorizationBroker.Authorize(logEntryBroker.GetAppId(internalLogEntry), "LogEntry_update");
        return ToExternalLogEntry(await logEntryBroker.UpdateLogEntryAsync(internalLogEntry));
    }

    public async ValueTask DeleteAsync(int id)
    {
        cCoder.Data.Models.Logging.LogEntry logEntry = Get(id);
        cCoder.Data.Models.Logging.LogEntry internalLogEntry = ToInternalLogEntry(logEntry);
        authorizationBroker.Authorize(logEntryBroker.GetAppId(internalLogEntry), "LogEntry_delete");
        await logEntryBroker.DeleteLogEntryAsync(internalLogEntry);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.Logging.LogEntry> items)
    {
        cCoder.Data.Models.Logging.LogEntry[] itemArray = items.ToArray();
        cCoder.Data.Models.Logging.LogEntry[] internalLogEntries = itemArray.Select(ToInternalLogEntry).ToArray();
        foreach (int appId in internalLogEntries.Select((cCoder.Data.Models.Logging.LogEntry item) => logEntryBroker.GetAppId(item)).Distinct())
        {
            authorizationBroker.Authorize(appId, "LogEntry_delete");
        }
        await logEntryBroker.DeleteAllLogEntriesAsync(internalLogEntries);
    }

    public ValueTask<int> DeleteEntriesBeforeAsync(DateTime cutoff) =>
        logEntryBroker.DeleteLogEntriesBeforeAsync(cutoff);

    public int? ResolveAppId(string domainOrName) =>
        logEntryBroker.GetAppId(domainOrName);

    public async ValueTask<IEnumerable<Result<cCoder.Data.Models.Logging.LogEntry>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.Logging.LogEntry> items)
    {
        List<Result<cCoder.Data.Models.Logging.LogEntry>> results = new List<Result<cCoder.Data.Models.Logging.LogEntry>>();

        foreach (cCoder.Data.Models.Logging.LogEntry item in items)
        {
            try
            {
                cCoder.Data.Models.Logging.LogEntry savedItem =
                    item.Id == 0
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<cCoder.Data.Models.Logging.LogEntry>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<cCoder.Data.Models.Logging.LogEntry>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private static cCoder.Data.Models.Logging.LogEntry ToExternalLogEntry(cCoder.Data.Models.Logging.LogEntry item)
    {
        cCoder.Data.Models.Logging.LogEntry logEntry = new cCoder.Data.Models.Logging.LogEntry();
        logEntry.Id = item.Id;
        logEntry.AppId = item.AppId;
        logEntry.Level = item.Level;
        logEntry.Message = item.Message;
        logEntry.AppName = item.AppName;
        logEntry.TypeName = item.TypeName;
        logEntry.Date = item.Date;
        logEntry.Data = item.Data?.Select(ToExternalLogDataItem).ToArray();
        return logEntry;
    }

    private static cCoder.Data.Models.Logging.LogDataItem ToExternalLogDataItem(cCoder.Data.Models.Logging.LogDataItem item)
    {
        return new cCoder.Data.Models.Logging.LogDataItem
        {
            Id = item.Id,
            LogEntryId = item.LogEntryId,
            Name = item.Name,
            Value = item.Value
        };
    }

    private static cCoder.Data.Models.Logging.LogEntry ToInternalLogEntry(cCoder.Data.Models.Logging.LogEntry item)
    {
        cCoder.Data.Models.Logging.LogEntry logEntry = new cCoder.Data.Models.Logging.LogEntry();
        logEntry.Id = item.Id;
        logEntry.AppId = item.AppId;
        logEntry.Level = item.Level;
        logEntry.Message = item.Message;
        logEntry.AppName = item.AppName;
        logEntry.TypeName = item.TypeName;
        logEntry.Date = item.Date;
        logEntry.Data = item.Data?.Select(ToInternalLogDataItem).ToArray();
        return logEntry;
    }

    private static cCoder.Data.Models.Logging.LogDataItem ToInternalLogDataItem(cCoder.Data.Models.Logging.LogDataItem item)
    {
        return new cCoder.Data.Models.Logging.LogDataItem
        {
            Id = item.Id,
            LogEntryId = item.LogEntryId,
            Name = item.Name,
            Value = item.Value
        };
    }
}
