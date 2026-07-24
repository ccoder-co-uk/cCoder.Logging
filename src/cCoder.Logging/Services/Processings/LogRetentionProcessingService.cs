// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryRetentionProcessingService(
    ILogEntryService logEntryService,
    LoggingConfiguration loggingConfiguration)
        : ILogEntryRetentionProcessingService
{
    public Task RunLogRetentionAsync(CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [cancellationToken]);

            while (!cancellationToken.IsCancellationRequested)
            {
                await DeleteExpiredLogEntries(
                    cancellationToken: cancellationToken);

                await Task.Delay(
                    delay: GetInterval(),
                    cancellationToken: cancellationToken);
            }
        });

    public ValueTask<int> DeleteExpiredLogEntriesAsync(
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [cancellationToken]);

            return await DeleteExpiredLogEntries(
                cancellationToken: cancellationToken);
        });

    private async ValueTask<int> DeleteExpiredLogEntries(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!loggingConfiguration.StoreLogEntries)
        {
            return 0;
        }

        DateTime cutoff = DateTime.UtcNow.AddDays(
            value: -GetRetentionDays());

        return await logEntryService.DeleteLogEntriesBeforeAsync(
            cutoff: cutoff);
    }

    private int GetRetentionDays() =>
        loggingConfiguration.RetentionDays <= 0
            ? 30
            : loggingConfiguration.RetentionDays;

    private TimeSpan GetInterval()
    {
        double minutes =
            loggingConfiguration.RetentionIntervalMinutes <= 0
                ? 60
                : loggingConfiguration.RetentionIntervalMinutes;

        return TimeSpan.FromMinutes(value: minutes);
    }
}