// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;
using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogHubProcessingService(
    ILogHubBroker logHubBroker,
    IAuthorizationBroker authorizationBroker,
    ILoggingBroker log) : ILogHubProcessingService
{
    private static readonly IDictionary<string, ICollection<HistoryItem>>
        History = new Dictionary<string, ICollection<HistoryItem>>();

    private static readonly IDictionary<string, int> UserCounts =
        new Dictionary<string, int>();

    public ValueTask ConnectLogHubSessionAsync(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            log.LogDebug(
                message: "New client connected to the logging hub.");

            return ValueTask.CompletedTask;
        });

    public ValueTask JoinLogHubSessionAsync(LogHubSession logHubSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            int? appId =
                logHubBroker.SelectAppIdByDomain(
                    domain: logHubSession.Thread);

            User user = authorizationBroker.SelectCurrentUser();

            if (appId.HasValue
                && user.IsAdminOfApp(appId: appId.Value))
            {
                await logHubBroker.JoinGroupAsync(logHubSession: logHubSession);

                await logHubBroker.SendCallerAsync(
                    logHubSession: logHubSession,
                    level: "info",
                    message: $"Connected to instance {logHubSession.Thread}");

                await logHubBroker.SendGroupAsync(
                    logHubSession: logHubSession,
                    level: "info",
                    message: "User Joined");

                log.LogInformation(
                    message:
                        "User {UserId} is listening to logs for {Domain}.",
                    args: [user.Id, logHubSession.Thread]);

                ICollection<HistoryItem> history =
                    GetOrCreateHistory(thread: logHubSession.Thread);

                UserCounts[logHubSession.Thread] =
                    GetUserCount(thread: logHubSession.Thread) + 1;

                foreach (HistoryItem item in history)
                {
                    await logHubBroker.SendCallerAsync(
                        logHubSession: logHubSession,
                        level: item.Level,
                        message: item.Message);
                }

                return;
            }

            log.LogWarning(
                message:
                    "User {UserId} was denied logging access to {Domain}.",
                args: [user.Id, logHubSession.Thread]);
        });

    public ValueTask LeaveLogHubSessionAsync(LogHubSession logHubSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            await logHubBroker.RemoveFromGroupAsync(logHubSession: logHubSession);

            await logHubBroker.SendCallerAsync(
                logHubSession: logHubSession,
                level: "info",
                message: $"Stopped listening to messages for {logHubSession.Thread}");

            await logHubBroker.SendGroupAsync(
                logHubSession: logHubSession,
                level: "info",
                message: "User Left");

            int userCount =
                GetUserCount(thread: logHubSession.Thread) - 1;

            UserCounts[logHubSession.Thread] = userCount;

            if (userCount <= 0)
            {
                _ = History.Remove(key: logHubSession.Thread);
                _ = UserCounts.Remove(key: logHubSession.Thread);
            }

            User user = authorizationBroker.SelectCurrentUser();

            log.LogInformation(
                message:
                    "User {UserId} stopped listening to logs for {Domain}.",
                args: [user.Id, logHubSession.Thread]);
        });

    public ValueTask DisconnectLogHubSessionAsync(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            User user = authorizationBroker.SelectCurrentUser();

            log.LogInformation(
                message: "User {UserId} disconnected.",
                args: [user.Id]);

            return ValueTask.CompletedTask;
        });

    public void DebugLogHubSession(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            logHubSession.Host = logHubBroker.SelectHost(logHubSession: logHubSession);

            log.LogDebug(
                message: "{Host}: {Level} {Message}",
                args: [logHubSession.Host, logHubSession.Level, logHubSession.Message]);
        });

    public void InfoLogHubSession(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            logHubSession.Host = logHubBroker.SelectHost(logHubSession: logHubSession);

            log.LogInformation(
                message: "{Host}: {Level} {Message}",
                args: [logHubSession.Host, logHubSession.Level, logHubSession.Message]);
        });

    public void WarnLogHubSession(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            logHubSession.Host = logHubBroker.SelectHost(logHubSession: logHubSession);

            log.LogWarning(
                message: "{Host}: {Level} {Message}",
                args: [logHubSession.Host, logHubSession.Level, logHubSession.Message]);
        });

    public void ErrorLogHubSession(LogHubSession logHubSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            logHubSession.Host = logHubBroker.SelectHost(logHubSession: logHubSession);

            log.LogError(
                message: "{Host}: {Level} {Message}",
                args: [logHubSession.Host, logHubSession.Level, logHubSession.Message]);
        });

    public ValueTask SendConsoleLogHubSessionAsync(
        LogHubSession logHubSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            ICollection<HistoryItem> history =
                GetOrCreateHistory(thread: logHubSession.Thread);

            history.Add(
                item: new HistoryItem
                {
                    Level = logHubSession.Level,
                    Message = logHubSession.Message,
                });

            await logHubBroker.SendGroupAsync(
                logHubSession: logHubSession,
                level: logHubSession.Level,
                message: logHubSession.Message);
        });

    public ValueTask SendTestLogHubSessionAsync(LogHubSession logHubSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logHubSession]);

            await logHubBroker.SendGroupAsync(
                logHubSession: logHubSession,
                level: "test",
                message: logHubSession.Message);
        });

    private static ICollection<HistoryItem> GetOrCreateHistory(
        string thread)
    {
        if (!History.TryGetValue(
            key: thread,
            value: out ICollection<HistoryItem> history))
        {
            history = [];
            History.Add(key: thread, value: history);
        }

        return history;
    }

    private static int GetUserCount(string thread) =>
        UserCounts.TryGetValue(
            key: thread,
            value: out int userCount)
            ? userCount
            : 0;
}