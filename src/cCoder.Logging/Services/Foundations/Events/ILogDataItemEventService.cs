using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

public interface ILogDataItemEventService
{
    ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity);
}









