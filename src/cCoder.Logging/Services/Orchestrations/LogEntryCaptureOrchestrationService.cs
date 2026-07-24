// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogEntryCaptureOrchestrationService(
    ILogEntryCaptureProcessingService logEntryCaptureProcessingService,
    ILogEntryEventProcessingService logEntryEventProcessingService)
        : ILogEntryCaptureOrchestrationService
{
    public ValueTask CaptureLogEntryAsync(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryCaptureRequest]);

            LogEntry savedLogEntry =
                await logEntryCaptureProcessingService.CaptureLogEntryAsync(
                    logEntryCaptureRequest: logEntryCaptureRequest);

            if (savedLogEntry is not null)
            {
                await logEntryEventProcessingService
                    .RaiseLogEntryAddEventAsync(
                        entity: savedLogEntry);
            }
        });
}