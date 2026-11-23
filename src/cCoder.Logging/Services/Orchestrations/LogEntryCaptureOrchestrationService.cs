// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;

namespace cCoder.Logging.Services.Orchestrations;

internal sealed partial class LogEntryCaptureOrchestrationService(
    ILogEntryCaptureProcessingService logEntryCaptureProcessingService,
    ILogEntryStreamProcessingService logEntryStreamProcessingService,
    ILogEntryEventProcessingService logEntryEventProcessingService)
        : ILogEntryCaptureOrchestrationService
{
    public ValueTask CaptureLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryCaptureRequest]);

            string streamThread = await logEntryStreamProcessingService
                .StreamLogEntryCaptureRequestAsync(
                    logEntryCaptureRequest: logEntryCaptureRequest);

            if (streamThread is not null)
            {
                await logEntryEventProcessingService.ForwardLogStreamEventAsync(
                    thread: streamThread,
                    level: logEntryCaptureRequest.Level.ToString()
                        .ToLowerInvariant(),
                    message: logEntryCaptureRequest.Message);
            }

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
                        logEntry: operation.Result);
            }
        });
}