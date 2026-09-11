// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations.Events;

internal interface ILogDataItemEventService
{
    ValueTask RaiseLogDataItemAddEventAsync(LogDataItem logDataItem);
    ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem logDataItem);
    ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem logDataItem);
}