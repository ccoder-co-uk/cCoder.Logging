// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogDataItemService(
    ILogDataItemBroker logDataItemBroker,
    IAuthorizationBroker authorizationBroker)
        : ILogDataItemService
{
    public LogDataItem GetLogDataItem(int logDataItemId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logDataItemId]);

            return SelectAllLogDataItems(ignoreFilters: false)
                .Where(predicate: logDataItem =>
                    logDataItem.Id == logDataItemId)
                .Select(selector: logDataItem =>
                    ToExternalLogDataItem(logDataItem: logDataItem))
                .FirstOrDefault();
        });

    public IQueryable<LogDataItem> GetAllLogDataItems(
        bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return SelectAllLogDataItems(ignoreFilters: ignoreFilters);
        });

    public ValueTask<LogDataItem> AddLogDataItemAsync(
        LogDataItem newLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newLogDataItem]);
            Authorize(
                logDataItem: newLogDataItem,
                privilege: "LogDataItem_create");

            return await logDataItemBroker.InsertLogDataItemAsync(
                newLogDataItem: newLogDataItem);
        });

    public ValueTask<LogDataItem> UpdateLogDataItemAsync(
        LogDataItem updatedLogDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedLogDataItem]);
            Authorize(
                logDataItem: updatedLogDataItem,
                privilege: "LogDataItem_update");

            return await logDataItemBroker.UpdateLogDataItemAsync(
                updatedLogDataItem: updatedLogDataItem);
        });

    public ValueTask DeleteLogDataItemAsync(int logDataItemId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItemId]);

            LogDataItem logDataItem =
                SelectLogDataItem(logDataItemId: logDataItemId);

            Authorize(
                logDataItem: logDataItem,
                privilege: "LogDataItem_delete");

            _ = await logDataItemBroker.DeleteLogDataItemAsync(
                deletedLogDataItem: logDataItem);
        });

    private IQueryable<LogDataItem> SelectAllLogDataItems(
        bool ignoreFilters) =>
        ignoreFilters
            ? logDataItemBroker.SelectAllLogDataItemsIgnoringFilters()
            : logDataItemBroker.SelectAllLogDataItems();

    private LogDataItem SelectLogDataItem(int logDataItemId) =>
        SelectAllLogDataItems(ignoreFilters: false)
            .Where(predicate: logDataItem =>
                logDataItem.Id == logDataItemId)
            .Select(selector: logDataItem =>
                ToExternalLogDataItem(logDataItem: logDataItem))
            .FirstOrDefault();

    private void Authorize(
        LogDataItem logDataItem,
        string privilege)
    {
        int? appId =
            logDataItemBroker.SelectAppIdByLogDataItem(
                logDataItem: logDataItem);

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
