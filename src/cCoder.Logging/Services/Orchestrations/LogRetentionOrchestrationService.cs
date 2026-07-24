// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal class LogRetentionOrchestrationService(
    ILogEntryProcessingService logEntryProcessingService,
    LoggingConfiguration configuration) : ILogRetentionOrchestrationService
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await DeleteExpiredAsync(cancellationToken);
            await Task.Delay(GetInterval(), cancellationToken);
        }
    }

    public ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        if (!configuration.StoreLogEntries)
            return new ValueTask<int>(0);

        DateTime cutoff = DateTime.UtcNow.AddDays(-GetRetentionDays());
        return logEntryProcessingService.DeleteEntriesBeforeAsync(cutoff);
    }

    private int GetRetentionDays() =>
        configuration.RetentionDays <= 0 ? 30 : configuration.RetentionDays;

    private TimeSpan GetInterval() =>
        TimeSpan.FromMinutes(configuration.RetentionIntervalMinutes <= 0
            ? 60
            : configuration.RetentionIntervalMinutes);
}