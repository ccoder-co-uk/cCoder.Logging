// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;

namespace cCoder.Logging.Models;

internal sealed class LogEntryCaptureOperation
{
    internal LogEntryCaptureRequest Request { get; set; }

    internal LogEntry Result { get; set; }
}