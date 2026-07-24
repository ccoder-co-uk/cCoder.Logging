// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Exposures.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Brokers;

public interface ILogEntryStreamBroker
{
    ValueTask StreamAsync(string thread, string level, string message);
}

internal class LogEntryStreamBroker(IServiceProvider serviceProvider) : ILogEntryStreamBroker
{
    public async ValueTask StreamAsync(string thread, string level, string message)
    {
        IHubContext<LogHub> hubContext = serviceProvider.GetService<IHubContext<LogHub>>();

        if (hubContext is null || string.IsNullOrWhiteSpace(thread))
            return;

        await hubContext.Clients
            .Group(thread)
            .SendAsync("ConsoleReceive", level, message, thread);
    }
}