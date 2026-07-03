using cCoder.Logging.Services.Orchestrations;

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
        ILogRetentionOrchestrationService logRetentionOrchestrationService =
            scope.ServiceProvider.GetRequiredService<ILogRetentionOrchestrationService>();

        await logRetentionOrchestrationService.RunAsync(stoppingToken);
    }
}
