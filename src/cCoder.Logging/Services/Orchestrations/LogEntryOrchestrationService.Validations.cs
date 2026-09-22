// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogEntryOrchestrationService
{
    private static void ValidateLogEntryOnGet(int logEntryId) =>
        Validate(inputs: logEntryId);

    private static void ValidateAllLogEntriesOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateLogEntryOnAdd(LogEntry newLogEntry) =>
        Validate(inputs: newLogEntry);

    private static void ValidateSystemLogEntryOnAdd(LogEntry newLogEntry) =>
        Validate(inputs: newLogEntry);

    private static void ValidateLogEntryOnUpdate(LogEntry updatedLogEntry) =>
        Validate(inputs: updatedLogEntry);

    private static void ValidateLogEntryOnDelete(int logEntryId) =>
        Validate(inputs: logEntryId);

    private static void ValidateOrUpdateLogEntryResultsOnAdd(
        IEnumerable<LogEntry> logEntries) =>
        Validate(inputs: logEntries);

    private static void ValidateAllLogEntryOnDelete(
        IEnumerable<LogEntry> deletedLogEntries) =>
        Validate(inputs: deletedLogEntries);

    private static void ValidateLogEntriesBeforeOnDelete(DateTime cutoff) =>
        Validate(inputs: cutoff);

    private static void ValidateAppOnResolve(string domainOrName) =>
        Validate(inputs: domainOrName);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}