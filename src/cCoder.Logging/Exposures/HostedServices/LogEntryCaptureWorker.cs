// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.Logging.Exposures.HostedServices;

internal sealed class LogEntryCaptureWorker(
    IRequestLogQueueCoordinator requestLogQueueCoordinator)
        : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        requestLogQueueCoordinator.RunAsync();

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        requestLogQueueCoordinator.Complete();
        await base.StopAsync(cancellationToken: cancellationToken);
    }
}