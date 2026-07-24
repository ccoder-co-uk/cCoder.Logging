// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryRetentionProcessingService
{
    Task RunLogRetentionAsync(CancellationToken cancellationToken);

    ValueTask<int> DeleteExpiredLogEntriesAsync(
        CancellationToken cancellationToken = default);
}