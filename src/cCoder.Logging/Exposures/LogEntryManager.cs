// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;

namespace cCoder.Logging.Exposures;

internal sealed class LogEntryManager(
    ILogEntryOrchestrationService logEntryOrchestrationService)
        : ILogEntryManager
{
    public LogEntry GetLogEntry(int logEntryId) =>
        logEntryOrchestrationService.GetLogEntry(
            logEntryId: logEntryId);

    public IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false) =>
        logEntryOrchestrationService.GetAllLogEntries(
            ignoreFilters: ignoreFilters);

    public ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry) =>
        logEntryOrchestrationService.AddLogEntryAsync(
            newLogEntry: newLogEntry);

    public ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry) =>
        logEntryOrchestrationService.AddSystemLogEntryAsync(
            newLogEntry: newLogEntry);

    public ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry) =>
        logEntryOrchestrationService.UpdateLogEntryAsync(
            updatedLogEntry: updatedLogEntry);

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        logEntryOrchestrationService.DeleteLogEntryAsync(
            logEntryId: logEntryId);

    public async ValueTask<IEnumerable<Result<LogEntry>>> AddOrUpdateLogEntriesAsync(
        IEnumerable<LogEntry> logEntries) =>
        (await logEntryOrchestrationService
                .AddOrUpdateLogEntryResultsAsync(
                    logEntries: logEntries))
        .Select(
            selector: ToResult);

    public ValueTask DeleteAllLogEntriesAsync(
        IEnumerable<LogEntry> deletedLogEntries) =>
        logEntryOrchestrationService.DeleteAllLogEntryAsync(
            deletedLogEntries: deletedLogEntries);

    private static Result<LogEntry> ToResult(
        OperationResult<LogEntry> operationResult) =>
        new()
        {
            Success = operationResult.Success,
            Message = operationResult.Message,
            Item = operationResult.Item
        };

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        logEntryOrchestrationService.DeleteLogEntriesBeforeAsync(
            cutoff: cutoff);

    public int? ResolveAppId(string domainOrName) =>
        logEntryOrchestrationService.ResolveAppId(
            domainOrName: domainOrName);
}