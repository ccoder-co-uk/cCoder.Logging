// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogDataItemService
{
    private static void ValidateLogDataItemOnGet(int logDataItemId) =>
        ValidationRulesEngine.Validate(inputs: [logDataItemId]);

    private static void ValidateAllLogDataItemsOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateLogDataItemOnAdd(object logDataItem) =>
        ValidationRulesEngine.Validate(inputs: [logDataItem]);

    private static void ValidateLogDataItemOnUpdate(object logDataItem) =>
        ValidationRulesEngine.Validate(inputs: [logDataItem]);

    private static void ValidateLogDataItemOnDelete(int logDataItemId) =>
        ValidationRulesEngine.Validate(inputs: [logDataItemId]);
}