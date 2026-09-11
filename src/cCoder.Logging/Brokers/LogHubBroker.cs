// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Logging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Logging.Brokers;

internal interface ILogHubBroker
{
    int? SelectAppIdByDomain(string domain);
    string SelectHost(LogHubSession logHubSession);
    Task JoinGroupAsync(LogHubSession logHubSession);
    Task RemoveFromGroupAsync(LogHubSession logHubSession);
    Task SendCallerAsync(LogHubSession logHubSession, string level, string message);
    Task SendGroupAsync(LogHubSession logHubSession, string level, string message);
}

internal sealed class LogHubBroker(
    ICoreContextFactory contextFactory) : ILogHubBroker
{
    public int? SelectAppIdByDomain(string domain)
    {
        using CoreDataContext context =
            contextFactory.CreateCoreContext();

        return context.Apps
            .IgnoreQueryFilters()
            .Where(predicate: app => app.Domain == domain)
            .Select(selector: app => (int?)app.Id)
            .FirstOrDefault();
    }

    public string SelectHost(LogHubSession logHubSession) =>
        logHubSession.Context.GetHttpContext()?.Request.Host.Value;

    public Task JoinGroupAsync(LogHubSession logHubSession) =>
        logHubSession.Groups.AddToGroupAsync(
            connectionId: logHubSession.ConnectionId,
            groupName: logHubSession.Thread);

    public Task RemoveFromGroupAsync(LogHubSession logHubSession) =>
        logHubSession.Groups.RemoveFromGroupAsync(
            connectionId: logHubSession.ConnectionId,
            groupName: logHubSession.Thread);

    public Task SendCallerAsync(
        LogHubSession logHubSession,
        string level,
        string message) =>
        logHubSession.Clients.Caller.SendAsync(
            method: "ConsoleReceive",
            arg1: level,
            arg2: message,
            arg3: logHubSession.Thread);

    public Task SendGroupAsync(
        LogHubSession logHubSession,
        string level,
        string message) =>
        logHubSession.Clients.Group(groupName: logHubSession.Thread)
            .SendAsync(
                method: "ConsoleReceive",
                arg1: level,
                arg2: message,
                arg3: logHubSession.Thread);
}