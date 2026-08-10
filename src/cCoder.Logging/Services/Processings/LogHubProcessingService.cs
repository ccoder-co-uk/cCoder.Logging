// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;
using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Models;
using Microsoft.AspNetCore.SignalR;

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

    public ValueTask ConnectLogHubSessionAsync(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            log.LogDebug(
                message: "New client connected to the logging hub.");

            return ValueTask.CompletedTask;
        });

    public ValueTask JoinLogHubSessionAsync(LogHubSession session) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [session]);

            int? appId =
                logHubBroker.SelectAppIdByDomain(
                    domain: session.Thread);

            User user = authorizationBroker.SelectCurrentUser();

            if (appId.HasValue
                && user.IsAdminOfApp(appId: appId.Value))
            {
                await session.Groups.AddToGroupAsync(
                    connectionId: session.ConnectionId,
                    groupName: session.Thread);

                await session.Clients.Caller.SendAsync(
                    method: "ConsoleReceive",
                    arg1: "info",
                    arg2: $"Connected to instance {session.Thread}",
                    arg3: session.Thread);

                await session.Clients.Group(
                    groupName: session.Thread)
                    .SendAsync(
                        method: "ConsoleReceive",
                        arg1: "info",
                        arg2: "User Joined",
                        arg3: session.Thread);

                log.LogInformation(
                    message:
                        "User {UserId} is listening to logs for {Domain}.",
                    args: [user.Id, session.Thread]);

                ICollection<HistoryItem> history =
                    GetOrCreateHistory(thread: session.Thread);

                UserCounts[session.Thread] =
                    GetUserCount(thread: session.Thread) + 1;

                foreach (HistoryItem item in history)
                {
                    await session.Clients.Caller.SendAsync(
                        method: "ConsoleReceive",
                        arg1: item.Level,
                        arg2: item.Message,
                        arg3: session.Thread);
                }

                return;
            }

            log.LogWarning(
                message:
                    "User {UserId} was denied logging access to {Domain}.",
                args: [user.Id, session.Thread]);
        });

    public ValueTask LeaveLogHubSessionAsync(LogHubSession session) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [session]);

            await session.Groups.RemoveFromGroupAsync(
                connectionId: session.ConnectionId,
                groupName: session.Thread);

            await session.Clients.Caller.SendAsync(
                method: "ConsoleReceive",
                arg1: "info",
                arg2:
                    $"Stopped listening to messages for {session.Thread}",
                arg3: session.Thread);

            await session.Clients.Group(
                groupName: session.Thread)
                .SendAsync(
                    method: "ConsoleReceive",
                    arg1: "info",
                    arg2: "User Left",
                    arg3: session.Thread);

            int userCount =
                GetUserCount(thread: session.Thread) - 1;

            UserCounts[session.Thread] = userCount;

            if (userCount <= 0)
            {
                _ = History.Remove(key: session.Thread);
                _ = UserCounts.Remove(key: session.Thread);
            }

            User user = authorizationBroker.SelectCurrentUser();

            log.LogInformation(
                message:
                    "User {UserId} stopped listening to logs for {Domain}.",
                args: [user.Id, session.Thread]);
        });

    public ValueTask DisconnectLogHubSessionAsync(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            User user = authorizationBroker.SelectCurrentUser();

            log.LogInformation(
                message: "User {UserId} disconnected.",
                args: [user.Id]);

            return ValueTask.CompletedTask;
        });

    public void DebugLogHubSession(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            log.LogDebug(
                message: "{Host}: {Level} {Message}",
                args: [session.Host, session.Level, session.Message]);
        });

    public void InfoLogHubSession(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            log.LogInformation(
                message: "{Host}: {Level} {Message}",
                args: [session.Host, session.Level, session.Message]);
        });

    public void WarnLogHubSession(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            log.LogWarning(
                message: "{Host}: {Level} {Message}",
                args: [session.Host, session.Level, session.Message]);
        });

    public void ErrorLogHubSession(LogHubSession session) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [session]);

            log.LogError(
                message: "{Host}: {Level} {Message}",
                args: [session.Host, session.Level, session.Message]);
        });

    public ValueTask SendConsoleLogHubSessionAsync(
        LogHubSession session) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [session]);

            ICollection<HistoryItem> history =
                GetOrCreateHistory(thread: session.Thread);

            history.Add(
                item: new HistoryItem
                {
                    Level = session.Level,
                    Message = session.Message,
                });

            await session.Clients.Group(
                groupName: session.Thread)
                .SendAsync(
                    method: "ConsoleReceive",
                    arg1: session.Level,
                    arg2: session.Message,
                    arg3: session.Thread);
        });

    public ValueTask SendTestLogHubSessionAsync(LogHubSession session) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [session]);

            await session.Clients.Group(
                groupName: session.Thread)
                .SendAsync(
                    method: "ConsoleReceive",
                    arg1: "test",
                    arg2: session.Message,
                    arg3: session.Thread);
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