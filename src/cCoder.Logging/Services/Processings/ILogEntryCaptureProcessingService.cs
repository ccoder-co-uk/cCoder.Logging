// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryCaptureProcessingService
{
    ValueTask<LogEntry> CaptureLogEntryAsync(
        LogEntryCaptureRequest logEntryCaptureRequest);
}