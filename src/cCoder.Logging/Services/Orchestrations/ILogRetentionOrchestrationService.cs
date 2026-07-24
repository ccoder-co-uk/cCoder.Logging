// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Services.Orchestrations;

public interface ILogRetentionOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken);

    ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);
}