// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogDataItemOrchestrationService
{
    private static void ValidateLogDataItemOnGet(int logDataItemId) =>
        Validate(inputs: logDataItemId);

    private static void ValidateAllLogDataItemsOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateLogDataItemOnAdd(LogDataItem newLogDataItem) =>
        Validate(inputs: newLogDataItem);

    private static void ValidateLogDataItemOnUpdate(LogDataItem updatedLogDataItem) =>
        Validate(inputs: updatedLogDataItem);

    private static void ValidateLogDataItemOnDelete(int logDataItemId) =>
        Validate(inputs: logDataItemId);

    private static void ValidateOrUpdateLogDataItemResultsOnAdd(
        IEnumerable<LogDataItem> logDataItems) =>
        Validate(inputs: logDataItems);

    private static void ValidateAllLogDataItemOnDelete(
        IEnumerable<LogDataItem> deletedLogDataItems) =>
        Validate(inputs: deletedLogDataItems);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}