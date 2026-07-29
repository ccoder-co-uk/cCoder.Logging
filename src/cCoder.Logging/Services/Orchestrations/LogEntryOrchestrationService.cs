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
            ValidateInputs(inputs: [logEntryId]);

            return logEntryProcessingService.GetLogEntry(
                logEntryId: logEntryId);
        });

    public IQueryable<LogEntry> GetAllLogEntries(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return logEntryProcessingService.GetAllLogEntries(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogEntry> AddLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogEntry]);

            LogEntry savedLogEntry =
                await logEntryProcessingService.AddLogEntryAsync(
                    newLogEntry: newLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryAddEventAsync(
                entity: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask<LogEntry> AddSystemLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogEntry]);

            LogEntry savedLogEntry =
                await logEntryProcessingService.AddSystemLogEntryAsync(
                    newLogEntry: newLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryAddEventAsync(
                entity: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask<LogEntry> UpdateLogEntryAsync(
        LogEntry updatedLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedLogEntry]);

            LogEntry savedLogEntry =
                await logEntryProcessingService.UpdateLogEntryAsync(
                    updatedLogEntry: updatedLogEntry);

            await logEntryEventProcessingService.RaiseLogEntryUpdateEventAsync(
                entity: savedLogEntry);

            return savedLogEntry;
        });

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryId]);

            LogEntry deletedLogEntry =
                logEntryProcessingService.GetLogEntry(
                    logEntryId: logEntryId);

            await logEntryEventProcessingService.RaiseLogEntryDeleteEventAsync(
                entity: deletedLogEntry);

            await logEntryProcessingService.DeleteLogEntryAsync(
                logEntryId: logEntryId);
        });

    public ValueTask<IEnumerable<OperationResult<LogEntry>>> AddOrUpdateLogEntryResultsAsync(
        IEnumerable<LogEntry> logEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntries]);

            return await logEntryProcessingService
                .AddOrUpdateLogEntryResultsAsync(
                    logEntries: logEntries);
        });

    public ValueTask DeleteAllLogEntryAsync(
        IEnumerable<LogEntry> deletedLogEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedLogEntries]);

            await logEntryProcessingService.DeleteAllLogEntryAsync(
                deletedLogEntries: deletedLogEntries);
        });

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [cutoff]);

            return await logEntryProcessingService.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);
        });

    public int? ResolveAppId(string domainOrName) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [domainOrName]);

            return logEntryProcessingService.ResolveAppId(
                domainOrName: domainOrName);
        });
}