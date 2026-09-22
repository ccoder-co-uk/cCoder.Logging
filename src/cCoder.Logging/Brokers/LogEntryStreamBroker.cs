// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Exposures.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Brokers;

internal interface ILogEntryStreamBroker
{
    ValueTask SendLogEntryAsync(
        string thread,
        string level,
        string message);
}

internal sealed class LogEntryStreamBroker(
    IServiceProvider serviceProvider)
        : ILogEntryStreamBroker
{
    public ValueTask SendLogEntryAsync(
        string thread,
        string level,
        string message) =>
        new(
            task: serviceProvider
                .GetService<IHubContext<LogHub>>()?
                .Clients
                .Group(groupName: thread)
                .SendAsync(
                    method: "ConsoleReceive",
                    arg1: level,
                    arg2: message,
                    arg3: thread)
                ?? Task.CompletedTask);
}