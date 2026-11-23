// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryEventProcessingService(
    ILogEntryEventService eventService,
    ILoggingBroker log,
    LoggingConfiguration configuration)
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

    public ValueTask ForwardLogStreamEventAsync(string thread, string level, string message) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [thread, level, message]);

            if (configuration.EventProviders?.OfType<EventProvider<LogStreamMessage>>()
                .Any(predicate: provider =>
                    provider.Events?.Contains(value: LogStreamMessage.EventName) == true) != true)
            {
                return;
            }

            try
            {
                await eventService.RaiseLogStreamEventAsync(
                    thread: thread,
                    level: level,
                    message: message);
            }
            catch (Exception exception)
            {
                log.LogError(
                    exception: exception,
                    message: "Unable to forward log stream message for {Domain}.",
                    args: thread);
            }
        });
}