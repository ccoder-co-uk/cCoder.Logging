// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;


namespace cCoder.Logging.Services.Foundations.Events;

internal class LogDataItemEventService(
    ILogDataItemEventBroker logDataItemEventBroker,
    ICoreAuthInfo authInfo
) : ILogDataItemEventService
{
    public async ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity)
    {
        EventMessage<LogDataItem> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logDataItemEventBroker.RaiseLogDataItemAddEventAsync(message);
    }

    public async ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity)
    {
        EventMessage<LogDataItem> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logDataItemEventBroker.RaiseLogDataItemUpdateEventAsync(message);
    }

    public async ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity)
    {
        EventMessage<LogDataItem> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logDataItemEventBroker.RaiseLogDataItemDeleteEventAsync(message);
    }
}