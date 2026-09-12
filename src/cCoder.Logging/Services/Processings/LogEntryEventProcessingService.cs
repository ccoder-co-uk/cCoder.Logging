// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryEventProcessingService(
    ILogEntryEventService eventService)
        : ILogEntryEventProcessingService
{
    public ValueTask RaiseLogEntryAddEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            await eventService.RaiseLogEntryAddEventAsync(
                logEntry: logEntry);
        });

    public ValueTask RaiseLogEntryUpdateEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            await eventService.RaiseLogEntryUpdateEventAsync(
                logEntry: logEntry);
        });

    public ValueTask RaiseLogEntryDeleteEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            await eventService.RaiseLogEntryDeleteEventAsync(
                logEntry: logEntry);
        });
}