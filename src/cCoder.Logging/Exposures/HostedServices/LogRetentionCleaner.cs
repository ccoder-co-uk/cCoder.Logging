// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Exposures.HostedServices;

public interface ILogRetentionCleaner : IHostedService
{
}

internal sealed class LogRetentionCleaner(
    ILogEntryRetentionProcessingService logRetentionProcessingService)
        : BackgroundService, ILogRetentionCleaner
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        logRetentionProcessingService.RunLogRetentionAsync(
            cancellationToken: stoppingToken);
}