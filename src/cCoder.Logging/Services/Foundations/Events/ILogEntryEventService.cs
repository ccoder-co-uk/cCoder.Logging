using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

public interface ILogEntryEventService
{
    ValueTask RaiseLogEntryAddEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity);
    ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity);
}









