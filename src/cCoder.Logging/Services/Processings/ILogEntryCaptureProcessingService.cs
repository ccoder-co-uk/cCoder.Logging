// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryCaptureProcessingService
{
    ValueTask<LogEntryCaptureOperation>
        CaptureLogEntryCaptureOperationAsync(
            LogEntryCaptureOperation operation);
}