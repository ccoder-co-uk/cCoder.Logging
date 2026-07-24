// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryService
{
    private static void ValidateLogEntryOnGet(int logEntryId) =>
        ValidationRulesEngine.Validate(inputs: [logEntryId]);

    private static void ValidateAllLogEntriesOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateLogEntryOnAdd(object logEntry) =>
        ValidationRulesEngine.Validate(inputs: [logEntry]);

    private static void ValidateSystemLogEntryOnAdd(object logEntry) =>
        ValidationRulesEngine.Validate(inputs: [logEntry]);

    private static void ValidateLogEntryOnUpdate(object logEntry) =>
        ValidationRulesEngine.Validate(inputs: [logEntry]);

    private static void ValidateLogEntryOnDelete(int logEntryId) =>
        ValidationRulesEngine.Validate(inputs: [logEntryId]);

    private static void ValidateLogEntriesBeforeOnDelete(DateTime cutoff) =>
        ValidationRulesEngine.Validate(inputs: [cutoff]);

    private static void ValidateAppOnResolve(string domainOrName) =>
        ValidationRulesEngine.Validate(inputs: [domainOrName]);
}