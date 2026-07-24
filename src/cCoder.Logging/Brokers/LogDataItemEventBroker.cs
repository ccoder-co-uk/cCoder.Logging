// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Logging.Brokers;

internal interface ILogDataItemEventBroker
{
    ValueTask RaiseLogDataItemAddEventAsync(EventMessage<LogDataItem> message);
    ValueTask RaiseLogDataItemUpdateEventAsync(EventMessage<LogDataItem> message);
    ValueTask RaiseLogDataItemDeleteEventAsync(EventMessage<LogDataItem> message);
}

internal sealed class LogDataItemEventBroker(IEventHub eventHub) : ILogDataItemEventBroker
{
    public ValueTask RaiseLogDataItemAddEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync(name: "log_data_item_add", message: message);

    public ValueTask RaiseLogDataItemUpdateEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync(name: "log_data_item_update", message: message);

    public ValueTask RaiseLogDataItemDeleteEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync(name: "log_data_item_delete", message: message);
}
