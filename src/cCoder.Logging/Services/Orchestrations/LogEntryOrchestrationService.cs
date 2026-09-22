// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogEntryOrchestrationService(
    ILogEntryProcessingService logEntryProcessingService,
    ILogEntryEventProcessingService logEntryEventProcessingService)
        : ILogEntryOrchestrationService
{
    public LogEntry GetLogEntry(int logEntryId) =>
        TryCatch(operation: () =>
        {
            ValidateLogEntryOnGet(logEntryId: logEntryId);

            return logEntryProcessingService.GetLogEntry(
                logEntryId: logEntryId);
        });

    public IQueryable<LogEntry> GetAllLogEntries(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllLogEntriesOnGet(ignoreFilters: ignoreFilters);

            return logEntryProcessingService.GetAllLogEntries(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogEntry> AddLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnAdd(newLogEntry: newLogEntry);

            LogEntry savedLogEntry =
                await logEntryProcessingService.AddLogEntryAsync(
                    newLogEntry: newLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryAddEventAsync(
                logEntry: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask<LogEntry> AddSystemLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateSystemLogEntryOnAdd(newLogEntry: newLogEntry);

            LogEntry savedLogEntry =
                await logEntryProcessingService.AddSystemLogEntryAsync(
                    newLogEntry: newLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryAddEventAsync(
                logEntry: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask<LogEntry> UpdateLogEntryAsync(
        LogEntry updatedLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnUpdate(updatedLogEntry: updatedLogEntry);

            LogEntry savedLogEntry =
                await logEntryProcessingService.UpdateLogEntryAsync(
                    updatedLogEntry: updatedLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryUpdateEventAsync(
                logEntry: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnDelete(logEntryId: logEntryId);

            LogEntry deletedLogEntry =
                logEntryProcessingService.GetLogEntry(
                    logEntryId: logEntryId);

            await logEntryEventProcessingService.RaiseLogEntryDeleteEventAsync(
                logEntry: deletedLogEntry);

            await logEntryProcessingService.DeleteLogEntryAsync(
                logEntryId: logEntryId);
        });

    public ValueTask<IEnumerable<OperationResult<LogEntry>>> AddOrUpdateLogEntryResultsAsync(
        IEnumerable<LogEntry> logEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateOrUpdateLogEntryResultsOnAdd(logEntries: logEntries);

            return await logEntryProcessingService
                .AddOrUpdateLogEntryResultsAsync(
                    logEntries: logEntries);
        });

    public ValueTask DeleteAllLogEntryAsync(
        IEnumerable<LogEntry> deletedLogEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateAllLogEntryOnDelete(
                deletedLogEntries: deletedLogEntries);

            await logEntryProcessingService.DeleteAllLogEntryAsync(
                deletedLogEntries: deletedLogEntries);
        });

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntriesBeforeOnDelete(cutoff: cutoff);

            return await logEntryProcessingService.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);
        });

    public int? ResolveAppId(string domainOrName) =>
        TryCatch(operation: () =>
        {
            ValidateAppOnResolve(domainOrName: domainOrName);

            return logEntryProcessingService.ResolveAppId(
                domainOrName: domainOrName);
        });
}