// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Exposures;

public interface ILogDataItemManager
{
    LogDataItem GetLogDataItem(int logDataItemId);
    IQueryable<LogDataItem> GetAllLogDataItems(bool ignoreFilters = false);
    ValueTask<LogDataItem> AddLogDataItemAsync(LogDataItem newLogDataItem);
    ValueTask<LogDataItem> UpdateLogDataItemAsync(LogDataItem updatedLogDataItem);
    ValueTask DeleteLogDataItemAsync(int logDataItemId);
    ValueTask<IEnumerable<Result<LogDataItem>>> AddOrUpdateLogDataItemsAsync(
        IEnumerable<LogDataItem> logDataItems);
    ValueTask DeleteAllLogDataItemsAsync(
        IEnumerable<LogDataItem> deletedLogDataItems);
}
