// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Processings;

internal interface ILogDataItemEventProcessingService
{
    ValueTask RaiseLogDataItemAddEventAsync(LogDataItem logDataItem);
    ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem logDataItem);
    ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem logDataItem);
}