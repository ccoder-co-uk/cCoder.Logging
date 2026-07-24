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
    public ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: entity);

            await logDataItemEventBroker.RaiseLogDataItemAddEventAsync(
                message: message);
        });

    public ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: entity);

            await logDataItemEventBroker.RaiseLogDataItemUpdateEventAsync(
                message: message);
        });

    public ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<LogDataItem> message =
                CreateLogDataItemEventMessage(logDataItem: entity);

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