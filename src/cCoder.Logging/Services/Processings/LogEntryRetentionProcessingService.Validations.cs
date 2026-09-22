// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryRetentionProcessingService
{
    private static void ValidateLogRetentionOnRun(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExpiredLogEntriesOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}