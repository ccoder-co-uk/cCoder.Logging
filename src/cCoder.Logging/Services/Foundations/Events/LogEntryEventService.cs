// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;

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

    public ValueTask RaiseLogStreamEventAsync(string thread, string level, string message) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [thread, level, message]);

            await logEntryEventBroker.RaiseLogStreamEventAsync(
                message: new EventMessage<LogStreamMessage>
                {
                    AuthInfo = new EventAuthInfo { SSOUserId = "Guest" },
                    Data = new LogStreamMessage
                    {
                        Domain = thread,
                        Level = level,
                        Message = message
                    }
                });
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