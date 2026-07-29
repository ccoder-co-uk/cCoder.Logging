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
            ValidateInputs(inputs: [logDataItemId]);

            return logDataItemProcessingService.GetLogDataItem(
                logDataItemId: logDataItemId);
        });

    public IQueryable<LogDataItem> GetAllLogDataItems(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return logDataItemProcessingService.GetAllLogDataItems(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogDataItem> AddLogDataItemAsync(
        LogDataItem newLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogDataItem]);

            LogDataItem savedLogDataItem =
                await logDataItemProcessingService.AddLogDataItemAsync(
                    newLogDataItem: newLogDataItem);

            await logDataItemEventProcessingService
                .RaiseLogDataItemAddEventAsync(
                    entity: savedLogDataItem);

            return savedLogDataItem;
        });

    public ValueTask<LogDataItem> UpdateLogDataItemAsync(
        LogDataItem updatedLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedLogDataItem]);

            LogDataItem savedLogDataItem =
                await logDataItemProcessingService.UpdateLogDataItemAsync(
                    updatedLogDataItem: updatedLogDataItem);

            await logDataItemEventProcessingService
                .RaiseLogDataItemUpdateEventAsync(
                    entity: savedLogDataItem);

            return savedLogDataItem;
        });

    public ValueTask DeleteLogDataItemAsync(int logDataItemId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItemId]);

            LogDataItem deletedLogDataItem =
                logDataItemProcessingService.GetLogDataItem(
                    logDataItemId: logDataItemId);

            await logDataItemEventProcessingService
                .RaiseLogDataItemDeleteEventAsync(
                    entity: deletedLogDataItem);

            await logDataItemProcessingService.DeleteLogDataItemAsync(
                logDataItemId: logDataItemId);
        });

    public ValueTask<IEnumerable<OperationResult<LogDataItem>>> AddOrUpdateLogDataItemResultsAsync(
        IEnumerable<LogDataItem> logDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItems]);

            return await logDataItemProcessingService
                .AddOrUpdateLogDataItemResultsAsync(
                    logDataItems: logDataItems);
        });

    public ValueTask DeleteAllLogDataItemAsync(
        IEnumerable<LogDataItem> deletedLogDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedLogDataItems]);

            await logDataItemProcessingService.DeleteAllLogDataItemAsync(
                deletedLogDataItems: deletedLogDataItems);
        });

}