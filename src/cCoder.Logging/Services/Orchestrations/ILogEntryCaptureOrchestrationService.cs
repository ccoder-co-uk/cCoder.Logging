// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Orchestrations;

public interface ILogEntryCaptureOrchestrationService
{
    ValueTask CaptureAsync(LogEntryCaptureRequest request);
}