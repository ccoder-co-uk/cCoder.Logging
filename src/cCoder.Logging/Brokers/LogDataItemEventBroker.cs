// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Logging.Brokers;

public interface ILogDataItemEventBroker
{
    ValueTask RaiseLogDataItemAddEventAsync(EventMessage<LogDataItem> message);
    ValueTask RaiseLogDataItemUpdateEventAsync(EventMessage<LogDataItem> message);
    ValueTask RaiseLogDataItemDeleteEventAsync(EventMessage<LogDataItem> message);
}

internal class LogDataItemEventBroker(IEventHub eventHub) : ILogDataItemEventBroker
{
    public ValueTask RaiseLogDataItemAddEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync("log_data_item_add", message);

    public ValueTask RaiseLogDataItemUpdateEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync("log_data_item_update", message);

    public ValueTask RaiseLogDataItemDeleteEventAsync(EventMessage<LogDataItem> message) =>
        eventHub.RaiseEventAsync("log_data_item_delete", message);
}