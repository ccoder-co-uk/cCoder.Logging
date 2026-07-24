// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal class LogDataItemEventProcessingService(ILogDataItemEventService eventService) : ILogDataItemEventProcessingService
{
    public ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity) => eventService.RaiseLogDataItemAddEventAsync(entity);

    public ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity) => eventService.RaiseLogDataItemUpdateEventAsync(entity);

    public ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity) => eventService.RaiseLogDataItemDeleteEventAsync(entity);
}