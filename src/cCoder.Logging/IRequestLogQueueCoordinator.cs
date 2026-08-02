// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging;

internal interface IRequestLogQueueCoordinator
{
    Task RunAsync();
    void Complete();
}