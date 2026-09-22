// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogDataItemOrchestrationService(
    ILogDataItemProcessingService logDataItemProcessingService,
    ILogDataItemEventProcessingService logDataItemEventProcessingService)
        : ILogDataItemOrchestrationService
{
    public LogDataItem GetLogDataItem(int logDataItemId) =>
        TryCatch(operation: () =>
        {
            ValidateLogDataItemOnGet(logDataItemId: logDataItemId);

            return logDataItemProcessingService.GetLogDataItem(
                logDataItemId: logDataItemId);
        });

    public IQueryable<LogDataItem> GetAllLogDataItems(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllLogDataItemsOnGet(ignoreFilters: ignoreFilters);

            return logDataItemProcessingService.GetAllLogDataItems(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogDataItem> AddLogDataItemAsync(
        LogDataItem newLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateLogDataItemOnAdd(newLogDataItem: newLogDataItem);

            LogDataItem savedLogDataItem =
                await logDataItemProcessingService.AddLogDataItemAsync(
                    newLogDataItem: newLogDataItem);

            await logDataItemEventProcessingService
                .RaiseLogDataItemAddEventAsync(
                    logDataItem: savedLogDataItem);

            return savedLogDataItem;
        });

    public ValueTask<LogDataItem> UpdateLogDataItemAsync(
        LogDataItem updatedLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateLogDataItemOnUpdate(updatedLogDataItem: updatedLogDataItem);

            LogDataItem savedLogDataItem =
                await logDataItemProcessingService.UpdateLogDataItemAsync(
                    updatedLogDataItem: updatedLogDataItem);

            await logDataItemEventProcessingService
                .RaiseLogDataItemUpdateEventAsync(
                    logDataItem: savedLogDataItem);

            return savedLogDataItem;
        });

    public ValueTask DeleteLogDataItemAsync(int logDataItemId) =>
        TryCatch(operation: async () =>
        {
            ValidateLogDataItemOnDelete(logDataItemId: logDataItemId);

            LogDataItem deletedLogDataItem =
                logDataItemProcessingService.GetLogDataItem(
                    logDataItemId: logDataItemId);

            await logDataItemEventProcessingService
                .RaiseLogDataItemDeleteEventAsync(
                    logDataItem: deletedLogDataItem);

            await logDataItemProcessingService.DeleteLogDataItemAsync(
                logDataItemId: logDataItemId);
        });

    public ValueTask<IEnumerable<OperationResult<LogDataItem>>> AddOrUpdateLogDataItemResultsAsync(
        IEnumerable<LogDataItem> logDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateOrUpdateLogDataItemResultsOnAdd(
                logDataItems: logDataItems);

            return await logDataItemProcessingService
                .AddOrUpdateLogDataItemResultsAsync(
                    logDataItems: logDataItems);
        });

    public ValueTask DeleteAllLogDataItemAsync(
        IEnumerable<LogDataItem> deletedLogDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateAllLogDataItemOnDelete(
                deletedLogDataItems: deletedLogDataItems);

            await logDataItemProcessingService.DeleteAllLogDataItemAsync(
                deletedLogDataItems: deletedLogDataItems);
        });

}