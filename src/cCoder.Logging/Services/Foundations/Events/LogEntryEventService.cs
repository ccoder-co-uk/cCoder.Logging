// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
using cCoder.Logging.Brokers;

namespace cCoder.Logging.Services.Foundations.Events;

internal sealed partial class LogEntryEventService(
    ILogEntryEventBroker logEntryEventBroker,
    IAuthInfoBroker authInfoBroker)
        : ILogEntryEventService
{
    public ValueTask RaiseLogEntryAddEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            EventMessage<LogEntry> message =
                CreateLogEntryEventMessage(logEntry: logEntry);

            await logEntryEventBroker.RaiseLogEntryAddEventAsync(
                message: message);
        });

    public ValueTask RaiseLogEntryUpdateEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            EventMessage<LogEntry> message =
                CreateLogEntryEventMessage(logEntry: logEntry);

            await logEntryEventBroker.RaiseLogEntryUpdateEventAsync(
                message: message);
        });

    public ValueTask RaiseLogEntryDeleteEventAsync(LogEntry logEntry) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logEntry]);

            EventMessage<LogEntry> message =
                CreateLogEntryEventMessage(logEntry: logEntry);

            await logEntryEventBroker.RaiseLogEntryDeleteEventAsync(
                message: message);
        });

    private EventMessage<LogEntry> CreateLogEntryEventMessage(
        LogEntry logEntry) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfoBroker.SelectCurrentSsoUserId(),
            },
            Data = logEntry,
        };
}