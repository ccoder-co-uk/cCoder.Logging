// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryProcessingService(
    ILogEntryService logEntryService)
        : ILogEntryProcessingService
{
    public LogEntry GetLogEntry(int logEntryId) =>
        TryCatch(operation: () =>
        {
            ValidateLogEntryOnGet(logEntryId: logEntryId);

            return logEntryService.GetLogEntry(
                logEntryId: logEntryId);
        });

    public IQueryable<LogEntry> GetAllLogEntries(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllLogEntriesOnGet(ignoreFilters: ignoreFilters);

            return logEntryService.GetAllLogEntries(
                ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogEntry> AddLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnAdd(newLogEntry: newLogEntry);

            return await AddLogEntry(
                newLogEntry: newLogEntry,
                isSystemLogEntry: false);
        });

    public ValueTask<LogEntry> AddSystemLogEntryAsync(
        LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateSystemLogEntryOnAdd(newLogEntry: newLogEntry);

            return await AddLogEntry(
                newLogEntry: newLogEntry,
                isSystemLogEntry: true);
        });

    public ValueTask<LogEntry> UpdateLogEntryAsync(
        LogEntry updatedLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnUpdate(updatedLogEntry: updatedLogEntry);

            LogEntry internalLogEntry =
                ToInternalLogEntry(logEntry: updatedLogEntry);

            LogEntry savedLogEntry =
                await logEntryService.UpdateLogEntryAsync(
                    updatedLogEntry: internalLogEntry);

            return ToExternalLogEntry(logEntry: savedLogEntry);
        });

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnDelete(logEntryId: logEntryId);

            await logEntryService.DeleteLogEntryAsync(
                logEntryId: logEntryId);
        });

    public ValueTask<IEnumerable<OperationResult<LogEntry>>> AddOrUpdateLogEntryResultsAsync(
        IEnumerable<LogEntry> logEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateOrUpdateLogEntryResultsOnAdd(logEntries: logEntries);

            List<OperationResult<LogEntry>> results = [];

            foreach (LogEntry logEntry in logEntries)
            {
                OperationResult<LogEntry> result =
                    await AddOrUpdateLogEntry(logEntry: logEntry);

                results.Add(item: result);
            }

            return results.AsEnumerable();
        });

    public ValueTask DeleteAllLogEntryAsync(
        IEnumerable<LogEntry> deletedLogEntries) =>
        TryCatch(operation: async () =>
        {
            ValidateAllLogEntryOnDelete(
                deletedLogEntries: deletedLogEntries);

            foreach (LogEntry deletedLogEntry in deletedLogEntries)
            {
                await logEntryService.DeleteLogEntryAsync(
                    logEntryId: deletedLogEntry.Id);
            }
        });

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntriesBeforeOnDelete(cutoff: cutoff);

            return await logEntryService.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);
        });

    public int? ResolveAppId(string domainOrName) =>
        TryCatch(operation: () =>
        {
            ValidateAppOnResolve(domainOrName: domainOrName);

            return logEntryService.ResolveAppId(
                domainOrName: domainOrName);
        });

    private async ValueTask<LogEntry> AddLogEntry(
        LogEntry newLogEntry,
        bool isSystemLogEntry)
    {
        LogEntry internalLogEntry =
            ToInternalLogEntry(logEntry: newLogEntry);

        LogEntry savedLogEntry = isSystemLogEntry
            ? await logEntryService.AddSystemLogEntryAsync(
                newLogEntry: internalLogEntry)
            : await logEntryService.AddLogEntryAsync(
                newLogEntry: internalLogEntry);

        return ToExternalLogEntry(logEntry: savedLogEntry);
    }

    private async ValueTask<OperationResult<LogEntry>> AddOrUpdateLogEntry(
        LogEntry logEntry)
    {
        try
        {
            bool isNewLogEntry = logEntry.Id == 0;

            LogEntry savedLogEntry = isNewLogEntry
                ? await AddLogEntry(
                    newLogEntry: logEntry,
                    isSystemLogEntry: false)
                : await UpdateLogEntry(
                    updatedLogEntry: logEntry);

            string message = isNewLogEntry
                ? "Added Successfully"
                : "Updated Successfully";

            return new OperationResult<LogEntry>
            {
                Success = true,
                Item = savedLogEntry,
                Message = message,
            };
        }
        catch (Exception exception)
        {
            return new OperationResult<LogEntry>
            {
                Success = false,
                Item = logEntry,
                Message = exception.Message,
            };
        }
    }

    private async ValueTask<LogEntry> UpdateLogEntry(
        LogEntry updatedLogEntry)
    {
        LogEntry internalLogEntry =
            ToInternalLogEntry(logEntry: updatedLogEntry);

        LogEntry savedLogEntry =
            await logEntryService.UpdateLogEntryAsync(
                updatedLogEntry: internalLogEntry);

        return ToExternalLogEntry(logEntry: savedLogEntry);
    }

    private static LogEntry ToExternalLogEntry(LogEntry logEntry) =>
        new()
        {
            Id = logEntry.Id,
            AppId = logEntry.AppId,
            Level = logEntry.Level,
            Message = logEntry.Message,
            AppName = logEntry.AppName,
            TypeName = logEntry.TypeName,
            Date = logEntry.Date,
            Data = logEntry.Data?
                .Select(selector: ToExternalLogDataItem)
                .ToArray(),
        };

    private static LogDataItem ToExternalLogDataItem(
        LogDataItem logDataItem) =>
        new()
        {
            Id = logDataItem.Id,
            LogEntryId = logDataItem.LogEntryId,
            Name = logDataItem.Name,
            Value = logDataItem.Value,
        };

    private static LogEntry ToInternalLogEntry(LogEntry logEntry) =>
        new()
        {
            Id = logEntry.Id,
            AppId = logEntry.AppId,
            Level = logEntry.Level,
            Message = logEntry.Message,
            AppName = logEntry.AppName,
            TypeName = logEntry.TypeName,
            Date = logEntry.Date,
            Data = logEntry.Data?
                .Select(selector: ToInternalLogDataItem)
                .ToArray(),
        };

    private static LogDataItem ToInternalLogDataItem(
        LogDataItem logDataItem) =>
        new()
        {
            Id = logDataItem.Id,
            LogEntryId = logDataItem.LogEntryId,
            Name = logDataItem.Name,
            Value = logDataItem.Value,
        };
}