// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;


namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryEventProcessingService(
    ILogEntryEventService eventService)
        : ILogEntryEventProcessingService
{
    public ValueTask RaiseLogEntryAddEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogEntryAddEventAsync(
                entity: entity);
        });

    public ValueTask RaiseLogEntryUpdateEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogEntryUpdateEventAsync(
                entity: entity);
        });

    public ValueTask RaiseLogEntryDeleteEventAsync(LogEntry entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            await eventService.RaiseLogEntryDeleteEventAsync(
                entity: entity);
        });
}
