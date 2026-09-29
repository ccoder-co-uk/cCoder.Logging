// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Logging;

internal interface IRequestLogQueueCoordinator
{
    Task RunAsync();
    void Complete();
}