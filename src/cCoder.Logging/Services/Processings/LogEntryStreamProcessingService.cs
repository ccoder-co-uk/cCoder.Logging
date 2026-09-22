// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryStreamProcessingService(
    ILogEntryStreamService logEntryStreamService)
        : ILogEntryStreamProcessingService
{
    public ValueTask StreamLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntryCaptureRequest]);

            string thread = FirstValue(
                values:
                [
                    logEntryCaptureRequest.RequestDomain,
                    logEntryStreamService.GetDefaultAppDomain(),
                    logEntryStreamService.GetDefaultAppId()?.ToString()
                ]);

            if (logEntryStreamService.ShouldStreamLogEntries()
                && !string.IsNullOrWhiteSpace(value: thread))
            {
                string level = logEntryCaptureRequest.Level
                    .ToString()
                    .ToLowerInvariant();

                await logEntryStreamService.StreamLogEntryAsync(
                    thread: thread,
                    level: level,
                    message: logEntryCaptureRequest.Message);
            }
        });

    private static string FirstValue(params string[] values) =>
        values.FirstOrDefault(
            predicate: value => !string.IsNullOrWhiteSpace(value: value));
}