// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryRetentionProcessingService
{
    Task RunLogRetentionAsync(CancellationToken cancellationToken);

    ValueTask<int> DeleteExpiredLogEntriesAsync(
        CancellationToken cancellationToken = default);
}