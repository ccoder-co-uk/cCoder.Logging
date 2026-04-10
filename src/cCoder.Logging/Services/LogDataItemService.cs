using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services;

internal class LogDataItemService(ILogDataItemBroker logDataItemBroker, IAuthorizationBroker authorizationBroker) : ILogDataItemService
{
    public cCoder.Data.Models.Logging.LogDataItem Get(int id)
    {
        return (from item in logDataItemBroker.GetAllLogDataItems(ignoreFilters: false)
                where item.Id == id
                select new cCoder.Data.Models.Logging.LogDataItem
                {
                    Id = item.Id,
                    LogEntryId = item.LogEntryId,
                    Name = item.Name,
                    Value = item.Value
                }).FirstOrDefault();
    }

    public IQueryable<cCoder.Data.Models.Logging.LogDataItem> GetAll(bool ignoreFilters = false)
    {
        return logDataItemBroker.GetAllLogDataItems(ignoreFilters);
    }

    public async ValueTask<cCoder.Data.Models.Logging.LogDataItem> AddAsync(cCoder.Data.Models.Logging.LogDataItem logDataItem)
    {
        cCoder.Data.Models.Logging.LogDataItem internalLogDataItem = ToInternalLogDataItem(logDataItem);
        authorizationBroker.Authorize(logDataItemBroker.GetAppId(internalLogDataItem), "LogDataItem_create");
        return ToExternalLogDataItem(await logDataItemBroker.AddLogDataItemAsync(internalLogDataItem));
    }

    public async ValueTask<cCoder.Data.Models.Logging.LogDataItem> UpdateAsync(cCoder.Data.Models.Logging.LogDataItem logDataItem)
    {
        cCoder.Data.Models.Logging.LogDataItem internalLogDataItem = ToInternalLogDataItem(logDataItem);
        authorizationBroker.Authorize(logDataItemBroker.GetAppId(internalLogDataItem), "LogDataItem_update");
        return ToExternalLogDataItem(await logDataItemBroker.UpdateLogDataItemAsync(internalLogDataItem));
    }

    public async ValueTask DeleteAsync(int id)
    {
        cCoder.Data.Models.Logging.LogDataItem logDataItem = Get(id);
        cCoder.Data.Models.Logging.LogDataItem internalLogDataItem = ToInternalLogDataItem(logDataItem);
        authorizationBroker.Authorize(logDataItemBroker.GetAppId(internalLogDataItem), "LogDataItem_delete");
        await logDataItemBroker.DeleteLogDataItemAsync(internalLogDataItem);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.Logging.LogDataItem> items)
    {
        cCoder.Data.Models.Logging.LogDataItem[] itemArray = items.ToArray();
        cCoder.Data.Models.Logging.LogDataItem[] internalLogDataItems = itemArray.Select(ToInternalLogDataItem).ToArray();
        foreach (int appId in internalLogDataItems.Select((cCoder.Data.Models.Logging.LogDataItem item) => logDataItemBroker.GetAppId(item)).Distinct())
        {
            authorizationBroker.Authorize(appId, "LogDataItem_delete");
        }
        await logDataItemBroker.DeleteAllLogDataItemsAsync(internalLogDataItems);
    }

    public async ValueTask<IEnumerable<Result<cCoder.Data.Models.Logging.LogDataItem>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.Logging.LogDataItem> items)
    {
        List<Result<cCoder.Data.Models.Logging.LogDataItem>> results = new List<Result<cCoder.Data.Models.Logging.LogDataItem>>();

        foreach (cCoder.Data.Models.Logging.LogDataItem item in items)
        {
            try
            {
                cCoder.Data.Models.Logging.LogDataItem savedItem =
                    item.Id == 0
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<cCoder.Data.Models.Logging.LogDataItem>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<cCoder.Data.Models.Logging.LogDataItem>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
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
