// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
using cCoder.Logging.Brokers;

namespace cCoder.Logging.Services.Foundations.Events;

internal sealed partial class LogDataItemEventService(
    ILogDataItemEventBroker logDataItemEventBroker,
    IAuthInfoBroker authInfoBroker)
        : ILogDataItemEventService
{
    public ValueTask RaiseLogDataItemAddEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: logDataItem);

            await logDataItemEventBroker.RaiseLogDataItemAddEventAsync(
                message: message);
        });

    public ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: logDataItem);

            await logDataItemEventBroker.RaiseLogDataItemUpdateEventAsync(
                message: message);
        });

    public ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: logDataItem);

            await logDataItemEventBroker.RaiseLogDataItemDeleteEventAsync(
                message: message);
        });

    private EventMessage<LogDataItem> CreateLogDataItemEventMessage(
        LogDataItem logDataItem) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfoBroker.SelectCurrentSsoUserId(),
            },
            Data = logDataItem,
        };
}