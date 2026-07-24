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
    public ValueTask RaiseLogEntryAddEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogEntry> message =
                CreateEventMessage(logEntry: entity);

            await logEntryEventBroker.RaiseLogEntryAddEventAsync(
                message: message);
        });

    public ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogEntry> message =
                CreateEventMessage(logEntry: entity);

            await logEntryEventBroker.RaiseLogEntryUpdateEventAsync(
                message: message);
        });

    public ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogEntry> message =
                CreateEventMessage(logEntry: entity);

            await logEntryEventBroker.RaiseLogEntryDeleteEventAsync(
                message: message);
        });

    private EventMessage<LogEntry> CreateEventMessage(
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
