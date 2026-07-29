// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Orchestrations;

internal interface ILogDataItemOrchestrationService
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