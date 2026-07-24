// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Exposures.HostedServices;

public interface ILogRetentionCleaner : IHostedService
{
}

internal sealed class LogRetentionCleaner(
    IServiceScopeFactory serviceScopeFactory) : BackgroundService, ILogRetentionCleaner
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        ILogEntryRetentionProcessingService logRetentionProcessingService =
            scope.ServiceProvider
                .GetRequiredService<ILogEntryRetentionProcessingService>();

        await logRetentionProcessingService.RunLogRetentionAsync(
            cancellationToken: stoppingToken);
    }
}