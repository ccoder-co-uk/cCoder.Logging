// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Exposures.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Brokers;

internal interface ILogEntryStreamBroker
{
    IHubContext<LogHub> SelectLogHubContext();
    ValueTask SendLogEntryAsync(
        IHubContext<LogHub> hubContext,
        string thread,
        string level,
        string message);
}

internal sealed class LogEntryStreamBroker(
    IServiceProvider serviceProvider)
        : ILogEntryStreamBroker
{
    public IHubContext<LogHub> SelectLogHubContext() =>
        serviceProvider.GetService<IHubContext<LogHub>>();

    public ValueTask SendLogEntryAsync(
        IHubContext<LogHub> hubContext,
        string thread,
        string level,
        string message) =>
        new(
            task: hubContext.Clients
                .Group(groupName: thread)
                .SendAsync(
                    method: "ConsoleReceive",
                    arg1: level,
                    arg2: message,
                    arg3: thread));
}