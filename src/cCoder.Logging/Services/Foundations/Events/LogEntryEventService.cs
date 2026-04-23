using cCoder.Data;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;


namespace cCoder.Logging.Services.Foundations.Events;

internal class LogEntryEventService(ILogEntryEventBroker logEntryEventBroker, ICoreAuthInfo authInfo)
    : ILogEntryEventService
{
    public async ValueTask RaiseLogEntryAddEventAsync(LogEntry entity)
    {
        EventMessage<LogEntry> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logEntryEventBroker.RaiseLogEntryAddEventAsync(message);
    }

    public async ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity)
    {
        EventMessage<LogEntry> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logEntryEventBroker.RaiseLogEntryUpdateEventAsync(message);
    }

    public async ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity)
    {
        EventMessage<LogEntry> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await logEntryEventBroker.RaiseLogEntryDeleteEventAsync(message);
    }
}








