// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogEntryCaptureOrchestrationService(
    ILogEntryCaptureProcessingService logEntryCaptureProcessingService,
    ILogEntryEventProcessingService logEntryEventProcessingService)
        : ILogEntryCaptureOrchestrationService
{
    public ValueTask CaptureLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryCaptureRequest]);

            LogEntryCaptureOperation operation =
                await logEntryCaptureProcessingService
                    .CaptureLogEntryCaptureOperationAsync(
                        operation: new LogEntryCaptureOperation
                        {
                            Request = logEntryCaptureRequest,
                        });

            if (operation.Result is not null)
            {
                await logEntryEventProcessingService
                    .RaiseLogEntryAddEventAsync(
                        entity: operation.Result);
            }
        });
}