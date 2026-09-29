// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;

using cCoder.Logging.Models;

namespace cCoder.Logging.Brokers;

internal interface ILogEntryCaptureQueue
{
    bool TryEnqueue(LogEntryCaptureRequest logEntryCaptureRequest);
    IAsyncEnumerable<LogEntryCaptureRequest> ReadAllAsync();
    void Complete();
}