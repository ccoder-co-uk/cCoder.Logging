// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Exposures.Hubs;

internal sealed class LogHub(
    ILogHubProcessingService processingService) : Hub
{
    public override Task OnConnectedAsync() =>
        processingService.ConnectLogHubSessionAsync(
            session: CreateLogHubSession())
        .AsTask();

    public Task Join(string thread) =>
        processingService.JoinLogHubSessionAsync(
            session: CreateLogHubSession(thread: thread))
        .AsTask();

    public Task Leave(string thread) =>
        processingService.LeaveLogHubSessionAsync(
            session: CreateLogHubSession(thread: thread))
        .AsTask();

    public override Task OnDisconnectedAsync(Exception exception) =>
        processingService.DisconnectLogHubSessionAsync(
            session: CreateLogHubSession(exception: exception))
        .AsTask();

    public void Debug(string level, string message) =>
        processingService.DebugLogHubSession(
            session: CreateLogHubSession(level: level, message: message));

    public void Info(string level, string message) =>
        processingService.InfoLogHubSession(
            session: CreateLogHubSession(level: level, message: message));

    public void Warn(string level, string message) =>
        processingService.WarnLogHubSession(
            session: CreateLogHubSession(level: level, message: message));

    public void Error(string level, string message) =>
        processingService.ErrorLogHubSession(
            session: CreateLogHubSession(level: level, message: message));

    public Task ConsoleSend(
        string level,
        string message,
        string thread) =>
        processingService.SendConsoleLogHubSessionAsync(
            session: CreateLogHubSession(
                thread: thread,
                level: level,
                message: message))
        .AsTask();

    public Task SendTest(string message, string thread) =>
        processingService.SendTestLogHubSessionAsync(
            session: CreateLogHubSession(
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
            Exception = exception,
            Groups = Groups,
            Host = Context.GetHttpContext()?.Request.Host.Value,
            Level = level,
            Message = message,
            Thread = thread,
        };
}