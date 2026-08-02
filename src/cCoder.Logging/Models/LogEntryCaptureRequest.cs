// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace cCoder.Logging.Models;

public sealed class LogEntryCaptureRequest
{
    public LogEntryCaptureRequest() => Persist = true;

    public LogLevel Level { get; set; }

    public string CategoryName { get; set; }

    public string Message { get; set; }

    public Exception Exception { get; set; }

    public string RequestDomain { get; set; }

    public bool Persist { get; set; }
}