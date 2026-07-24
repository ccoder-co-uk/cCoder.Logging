// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Dependencies;

namespace cCoder.Logging.Services.Processings;

internal interface ILogDataItemProcessingService
{
    LogDataItem GetLogDataItem(int logDataItemId);
    IQueryable<LogDataItem> GetAllLogDataItems(bool ignoreFilters = false);
    ValueTask<LogDataItem> AddLogDataItemAsync(LogDataItem newLogDataItem);
    ValueTask<LogDataItem> UpdateLogDataItemAsync(LogDataItem updatedLogDataItem);
    ValueTask DeleteLogDataItemAsync(int logDataItemId);
    ValueTask<IEnumerable<OperationResult<LogDataItem>>> AddOrUpdateLogDataItemResultsAsync(
        IEnumerable<LogDataItem> logDataItems);
    ValueTask DeleteAllLogDataItemAsync(
        IEnumerable<LogDataItem> deletedLogDataItems);
}