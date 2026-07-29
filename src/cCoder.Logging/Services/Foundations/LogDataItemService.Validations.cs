// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogDataItemService
{
    private static void ValidateLogDataItemOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllLogDataItemsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogDataItemOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogDataItemOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogDataItemOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}