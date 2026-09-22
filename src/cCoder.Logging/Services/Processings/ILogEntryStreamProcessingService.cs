// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryStreamProcessingService
{
    ValueTask StreamLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest);
}