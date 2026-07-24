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
    public ValueTask RaiseLogDataItemAddEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogDataItemAddEventAsync(
                entity: entity);
        });

    public ValueTask RaiseLogDataItemUpdateEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogDataItemUpdateEventAsync(
                entity: entity);
        });

    public ValueTask RaiseLogDataItemDeleteEventAsync(LogDataItem entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogDataItemDeleteEventAsync(
                entity: entity);
        });
}