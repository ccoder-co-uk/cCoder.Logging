// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;

namespace cCoder.Logging.Exposures;

internal sealed class LogDataItemManager(
    ILogDataItemOrchestrationService logDataItemOrchestrationService)
        : ILogDataItemManager
{
    public LogDataItem GetLogDataItem(int logDataItemId) =>
        logDataItemOrchestrationService.GetLogDataItem(
            logDataItemId: logDataItemId);

    public IQueryable<LogDataItem> GetAllLogDataItems(bool ignoreFilters = false) =>
        logDataItemOrchestrationService.GetAllLogDataItems(
            ignoreFilters: ignoreFilters);

    public ValueTask<LogDataItem> AddLogDataItemAsync(LogDataItem newLogDataItem) =>
        logDataItemOrchestrationService.AddLogDataItemAsync(
            newLogDataItem: newLogDataItem);

    public ValueTask<LogDataItem> UpdateLogDataItemAsync(
        LogDataItem updatedLogDataItem) =>
        logDataItemOrchestrationService.UpdateLogDataItemAsync(
            updatedLogDataItem: updatedLogDataItem);

    public ValueTask DeleteLogDataItemAsync(int logDataItemId) =>
        logDataItemOrchestrationService.DeleteLogDataItemAsync(
            logDataItemId: logDataItemId);

    public ValueTask<IEnumerable<Result<LogDataItem>>> AddOrUpdateLogDataItemsAsync(
        IEnumerable<LogDataItem> logDataItems) =>
        logDataItemOrchestrationService.AddOrUpdateLogDataItemsAsync(
            logDataItems: logDataItems);

    public ValueTask DeleteAllLogDataItemsAsync(
        IEnumerable<LogDataItem> deletedLogDataItems) =>
        logDataItemOrchestrationService.DeleteAllLogDataItemsAsync(
            deletedLogDataItems: deletedLogDataItems);
}
