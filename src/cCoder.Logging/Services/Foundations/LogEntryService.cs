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
            ValidateInputs(inputs: [logEntryId]);

            return GetAllLogEntries(ignoreFilters: false)
                .Where(predicate: logEntry => logEntry.Id == logEntryId)
                .Select(selector: logEntry => ToExternalLogEntry(logEntry: logEntry))
                .FirstOrDefault();
        });

    public IQueryable<LogEntry> GetAllLogEntries(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            IQueryable<LogEntry> logEntries = ignoreFilters
                ? logEntryBroker.SelectAllLogEntriesIgnoringFilters()
                : logEntryBroker.SelectAllLogEntries();

            return logEntries;
        });

    public ValueTask<LogEntry> AddLogEntryAsync(LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogEntry]);
            Authorize(logEntry: newLogEntry, privilege: "LogEntry_create");

            return await logEntryBroker.InsertLogEntryAsync(
                newLogEntry: newLogEntry);
        });

    public ValueTask<LogEntry> AddSystemLogEntryAsync(LogEntry newLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogEntry]);

            return await logEntryBroker.InsertLogEntryAsync(
                newLogEntry: newLogEntry);
        });

    public ValueTask<LogEntry> UpdateLogEntryAsync(LogEntry updatedLogEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedLogEntry]);
            Authorize(logEntry: updatedLogEntry, privilege: "LogEntry_update");

            return await logEntryBroker.UpdateLogEntryAsync(
                updatedLogEntry: updatedLogEntry);
        });

    public ValueTask DeleteLogEntryAsync(int logEntryId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryId]);

            LogEntry logEntry = GetLogEntry(logEntryId: logEntryId);
            Authorize(logEntry: logEntry, privilege: "LogEntry_delete");

            _ = await logEntryBroker.DeleteLogEntryAsync(
                deletedLogEntry: logEntry);
        });

    public ValueTask<int> DeleteLogEntriesBeforeAsync(DateTime cutoff) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [cutoff]);

            return await logEntryBroker.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);
        });

    public int? ResolveAppId(string domainOrName) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [domainOrName]);

            return logEntryBroker.SelectAppIdByDomainOrName(
                domainOrName: domainOrName);
        });

    private void Authorize(
        LogEntry logEntry,
        string privilege)
    {
        int? appId = logEntry.AppId > 0
            ? logEntry.AppId
            : ResolveAppId(domainOrName: logEntry.AppName);

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
            && role.Role.Allows(user, "app_admin"))
            ?? false);

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
