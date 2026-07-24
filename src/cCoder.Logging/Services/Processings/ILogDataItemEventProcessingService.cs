// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Processings;

public interface ILogDataItemEventProcessingService
{
    ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity);
    ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity);
}