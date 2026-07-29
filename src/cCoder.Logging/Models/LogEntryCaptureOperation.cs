// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Models;

internal sealed class LogEntryCaptureOperation
{
    internal LogEntryCaptureRequest Request { get; set; }

    internal LogEntry Result { get; set; }
}