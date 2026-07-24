// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

internal interface ILogDataItemEventService
{
    ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity);
}
