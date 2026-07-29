// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryService
{
    private static void ValidateLogEntryOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllLogEntriesOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogEntryOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSystemLogEntryOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogEntryOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogEntryOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogEntriesBeforeOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnResolve(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}