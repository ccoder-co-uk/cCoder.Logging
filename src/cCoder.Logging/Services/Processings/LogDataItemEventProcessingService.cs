// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogDataItemEventProcessingService(
    ILogDataItemEventService eventService)
        : ILogDataItemEventProcessingService
{
    public ValueTask RaiseLogDataItemAddEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            await eventService.RaiseLogDataItemAddEventAsync(
                logDataItem: logDataItem);
        });

    public ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            await eventService.RaiseLogDataItemUpdateEventAsync(
                logDataItem: logDataItem);
        });

    public ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem logDataItem) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [logDataItem]);

            await eventService.RaiseLogDataItemDeleteEventAsync(
                logDataItem: logDataItem);
        });
}