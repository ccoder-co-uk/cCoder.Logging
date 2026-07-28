// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryCaptureProcessingService
{
    ValueTask<LogEntryCaptureOperation>
        CaptureLogEntryCaptureOperationAsync(
            LogEntryCaptureOperation operation);
}