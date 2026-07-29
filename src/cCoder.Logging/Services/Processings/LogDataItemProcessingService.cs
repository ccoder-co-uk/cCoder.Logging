// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogDataItemProcessingService(
    ILogDataItemService logDataItemService)
        : ILogDataItemProcessingService
{
    public LogDataItem GetLogDataItem(int logDataItemId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logDataItemId]);

            return logDataItemService.GetLogDataItem(
                logDataItemId: logDataItemId);
        });

    public IQueryable<LogDataItem> GetAllLogDataItems(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return logDataItemService.GetAllLogDataItems(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogDataItem> AddLogDataItemAsync(
        LogDataItem newLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogDataItem]);

            return await AddLogDataItem(
                newLogDataItem: newLogDataItem);
        });

    public ValueTask<LogDataItem> UpdateLogDataItemAsync(
        LogDataItem updatedLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedLogDataItem]);

            return await UpdateLogDataItem(
                updatedLogDataItem: updatedLogDataItem);
        });

    public ValueTask DeleteLogDataItemAsync(int logDataItemId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItemId]);

            await logDataItemService.DeleteLogDataItemAsync(
                logDataItemId: logDataItemId);
        });

    public ValueTask<IEnumerable<OperationResult<LogDataItem>>> AddOrUpdateLogDataItemResultsAsync(
        IEnumerable<LogDataItem> logDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItems]);

            List<OperationResult<LogDataItem>> results = [];

            foreach (LogDataItem logDataItem in logDataItems)
            {
                OperationResult<LogDataItem> result =
                    await AddOrUpdateLogDataItem(
                        logDataItem: logDataItem);

                results.Add(item: result);
            }

            return results.AsEnumerable();
        });

    public ValueTask DeleteAllLogDataItemAsync(
        IEnumerable<LogDataItem> deletedLogDataItems) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedLogDataItems]);

            foreach (LogDataItem deletedLogDataItem in deletedLogDataItems)
            {
                await logDataItemService.DeleteLogDataItemAsync(
                    logDataItemId: deletedLogDataItem.Id);
            }
        });

    private async ValueTask<LogDataItem> AddLogDataItem(
        LogDataItem newLogDataItem)
    {
        LogDataItem internalLogDataItem =
            ToInternalLogDataItem(logDataItem: newLogDataItem);

        LogDataItem savedLogDataItem =
            await logDataItemService.AddLogDataItemAsync(
                newLogDataItem: internalLogDataItem);

        return ToExternalLogDataItem(logDataItem: savedLogDataItem);
    }

    private async ValueTask<OperationResult<LogDataItem>> AddOrUpdateLogDataItem(
        LogDataItem logDataItem)
    {
        try
        {
            bool isNewLogDataItem = logDataItem.Id == 0;

            LogDataItem savedLogDataItem = isNewLogDataItem
                ? await AddLogDataItem(
                    newLogDataItem: logDataItem)
                : await UpdateLogDataItem(
                    updatedLogDataItem: logDataItem);

            string message = isNewLogDataItem
                ? "Added Successfully"
                : "Updated Successfully";

            return new OperationResult<LogDataItem>
            {
                Success = true,
                Item = savedLogDataItem,
                Message = message,
            };
        }
        catch (Exception exception)
        {
            return new OperationResult<LogDataItem>
            {
                Success = false,
                Item = logDataItem,
                Message = exception.Message,
            };
        }
    }

    private async ValueTask<LogDataItem> UpdateLogDataItem(
        LogDataItem updatedLogDataItem)
    {
        LogDataItem internalLogDataItem =
            ToInternalLogDataItem(logDataItem: updatedLogDataItem);

        LogDataItem savedLogDataItem =
            await logDataItemService.UpdateLogDataItemAsync(
                updatedLogDataItem: internalLogDataItem);

        return ToExternalLogDataItem(logDataItem: savedLogDataItem);
    }

    private static LogDataItem ToExternalLogDataItem(
        LogDataItem logDataItem) =>
        new()
        {
            Id = logDataItem.Id,
            LogEntryId = logDataItem.LogEntryId,
            Name = logDataItem.Name,
            Value = logDataItem.Value,
        };

    private static LogDataItem ToInternalLogDataItem(
        LogDataItem logDataItem) =>
        new()
        {
            Id = logDataItem.Id,
            LogEntryId = logDataItem.LogEntryId,
            Name = logDataItem.Name,
            Value = logDataItem.Value,
        };
}