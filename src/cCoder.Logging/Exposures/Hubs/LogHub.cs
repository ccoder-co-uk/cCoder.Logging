// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Exposures.Hubs;

internal sealed class LogHub(
    ILogHubService logHubService) : Hub
{
    public override Task OnConnectedAsync() =>
        logHubService.ConnectLogHubSessionAsync(
            logHubSession: CreateLogHubSession())
        .AsTask();

    public Task Join(string thread) =>
        logHubService.JoinLogHubSessionAsync(
            logHubSession: CreateLogHubSession(thread: thread))
        .AsTask();

    public Task Leave(string thread) =>
        logHubService.LeaveLogHubSessionAsync(
            logHubSession: CreateLogHubSession(thread: thread))
        .AsTask();

    public override Task OnDisconnectedAsync(Exception exception) =>
        logHubService.DisconnectLogHubSessionAsync(
            logHubSession: CreateLogHubSession(exception: exception))
        .AsTask();

    public void Debug(string level, string message) =>
        logHubService.DebugLogHubSession(
            logHubSession: CreateLogHubSession(level: level, message: message));

    public void Info(string level, string message) =>
        logHubService.InfoLogHubSession(
            logHubSession: CreateLogHubSession(level: level, message: message));

    public void Warn(string level, string message) =>
        logHubService.WarnLogHubSession(
            logHubSession: CreateLogHubSession(level: level, message: message));

    public void Error(string level, string message) =>
        logHubService.ErrorLogHubSession(
            logHubSession: CreateLogHubSession(level: level, message: message));

    public Task ConsoleSend(
        string level,
        string message,
        string thread) =>
        logHubService.SendConsoleLogHubSessionAsync(
            logHubSession: CreateLogHubSession(
                thread: thread,
                level: level,
                message: message))
        .AsTask();

    public Task SendTest(string message, string thread) =>
        logHubService.SendTestLogHubSessionAsync(
            logHubSession: CreateLogHubSession(
                thread: thread,
                message: message))
        .AsTask();

    private LogHubSession CreateLogHubSession(
        string thread = null,
        string level = null,
        string message = null,
        Exception exception = null) =>
        new()
        {
            Clients = Clients,
            ConnectionId = Context.ConnectionId,
            Context = Context,
            Exception = exception,
            Groups = Groups,
            Level = level,
            Message = message,
            Thread = thread,
        };
}