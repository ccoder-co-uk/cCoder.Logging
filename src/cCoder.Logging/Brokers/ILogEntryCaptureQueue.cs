// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;

namespace cCoder.Logging.Brokers;

internal interface ILogEntryCaptureQueue
{
    bool TryEnqueue(LogEntryCaptureRequest request);
    IAsyncEnumerable<LogEntryCaptureRequest> ReadAllAsync();
    void Complete();
}