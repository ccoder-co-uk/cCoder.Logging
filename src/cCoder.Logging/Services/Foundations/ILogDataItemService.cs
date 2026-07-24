// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Foundations;

internal interface ILogDataItemService
{
    LogDataItem GetLogDataItem(int logDataItemId);
    IQueryable<LogDataItem> GetAllLogDataItems(bool ignoreFilters = false);
    ValueTask<LogDataItem> AddLogDataItemAsync(LogDataItem newLogDataItem);
    ValueTask<LogDataItem> UpdateLogDataItemAsync(LogDataItem updatedLogDataItem);
    ValueTask DeleteLogDataItemAsync(int logDataItemId);
}
