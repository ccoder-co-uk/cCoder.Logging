// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Orchestrations;

internal interface ILogEntryCaptureOrchestrationService
{
    ValueTask CaptureLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest);
}