// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;

namespace cCoder.Logging.Models;

internal sealed class LogHubSession
{
    internal IHubCallerClients Clients { get; set; }

    internal string ConnectionId { get; set; }

    internal Exception Exception { get; set; }

    internal IGroupManager Groups { get; set; }

    internal string Host { get; set; }

    internal string Level { get; set; }

    internal string Message { get; set; }

    internal string Thread { get; set; }
}