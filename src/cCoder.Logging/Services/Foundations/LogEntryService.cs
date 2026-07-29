// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryService(
    ILogEntryBroker logEntryBroker,
    IAuthorizationBroker authorizationBroker)
        : ILogEntryService
{
    public LogEntry GetLogEntry(int logEntryId) =>
        TryCatch(operation: () =>
        {
            ValidateLogEntryOnGet(inputs: [logEntryId]);

            return SelectAllLogEntries(ignoreFilters: false)
                .Where(predicate: logEntry => logEntry.Id == logEntryId)
                .Select(selector: logEntry => ToExternalLogEntry(logEntry: logEntry))
                .FirstOrDefault();
        });

    public IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllLogEntriesOnGet(inputs: [ignoreFilters]);

            return SelectAllLogEntries(ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnAdd(inputs: [newLogEntry]);
            Authorize(logEntry: newLogEntry, privilege: "LogEntry_create");

            LogEntry flatLogEntry = ToFlatLogEntry(logEntry: newLogEntry);

            LogEntry savedLogEntry =
                await logEntryBroker.InsertLogEntryAsync(
                    newLogEntry: flatLogEntry);

            newLogEntry.Id = savedLogEntry.Id;

            return newLogEntry;
        });

    public ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateSystemLogEntryOnAdd(inputs: [newLogEntry]);

            LogEntry flatLogEntry = ToFlatLogEntry(logEntry: newLogEntry);

            LogEntry savedLogEntry =
                await logEntryBroker.InsertLogEntryAsync(
                    newLogEntry: flatLogEntry);

            newLogEntry.Id = savedLogEntry.Id;

            return newLogEntry;
        });

    public ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnUpdate(inputs: [updatedLogEntry]);
            Authorize(logEntry: updatedLogEntry, privilege: "LogEntry_update");

            LogEntry flatLogEntry = ToFlatLogEntry(logEntry: updatedLogEntry);

            LogEntry savedLogEntry =
                await logEntryBroker.UpdateLogEntryAsync(
                    updatedLogEntry: flatLogEntry);

            updatedLogEntry.Id = savedLogEntry.Id;

            return updatedLogEntry;
        });

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntryOnDelete(inputs: [logEntryId]);

            LogEntry logEntry = SelectLogEntry(logEntryId: logEntryId);
            Authorize(logEntry: logEntry, privilege: "LogEntry_delete");

            _ = await logEntryBroker.DeleteLogEntryAsync(
                deletedLogEntry: logEntry);
        });

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        TryCatch(operation: async () =>
        {
            ValidateLogEntriesBeforeOnDelete(inputs: [cutoff]);

            return await logEntryBroker.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);
        });

    public int? ResolveAppId(string domainOrName) =>
        TryCatch(operation: () =>
        {
            ValidateAppOnResolve(inputs: [domainOrName]);

            return logEntryBroker.SelectAppIdByDomainOrName(
                domainOrName: domainOrName);
        });

    private void Authorize(
        LogEntry logEntry,
        string privilege)
    {
        int? appId = logEntry.AppId > 0
            ? logEntry.AppId
            : logEntryBroker.SelectAppIdByDomainOrName(
                domainOrName: logEntry.AppName);

        User user = authorizationBroker.SelectCurrentUser();

        bool isAuthorized =
            user is not null
            && (HasAppAdminPrivilege(user: user, appId: appId)
                || HasPrivilege(
                    user: user,
                    appId: appId,
                    privilege: privilege));

        if (!isAuthorized)
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static bool HasPrivilege(
        User user,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLowerInvariant();

        return (appId.HasValue
                && HasAppAdminPrivilege(user: user, appId: appId))
            || (user.Roles?.Any(predicate: role =>
                (!appId.HasValue || role.Role.AppId == appId)
                && role.Role.Privileges.Contains(value: normalizedPrivilege))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(
        User user,
        int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(predicate: role =>
            role.Role.AppId == appId.Value
            && role.Role.Allows(user: user, privilege: "app_admin"))
            ?? false);

    private IQueryable<LogEntry> SelectAllLogEntries(
        bool ignoreFilters) =>
        ignoreFilters
            ? logEntryBroker.SelectAllLogEntriesIgnoringFilters()
            : logEntryBroker.SelectAllLogEntries();

    private LogEntry SelectLogEntry(int logEntryId) =>
        SelectAllLogEntries(ignoreFilters: false)
            .Where(predicate: logEntry => logEntry.Id == logEntryId)
            .Select(selector: logEntry =>
                ToExternalLogEntry(logEntry: logEntry))
            .FirstOrDefault();

    private static LogEntry ToFlatLogEntry(LogEntry logEntry) =>
        new()
        {
            Id = logEntry.Id,
            AppId = logEntry.AppId,
            Level = logEntry.Level,
            Message = logEntry.Message,
            AppName = logEntry.AppName,
            TypeName = logEntry.TypeName,
            Date = logEntry.Date
        };

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
}