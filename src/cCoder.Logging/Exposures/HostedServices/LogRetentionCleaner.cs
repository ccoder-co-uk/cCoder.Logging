// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies.HostedServices;
using Microsoft.Extensions.Hosting;

namespace cCoder.Logging.Exposures.HostedServices;

public interface ILogRetentionCleaner : IHostedService
{
}

internal sealed class LogRetentionCleaner(
    LogRetentionRunner runLogRetentionAsync)
        : BackgroundService, ILogRetentionCleaner
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        runLogRetentionAsync(stoppingToken: stoppingToken);
}