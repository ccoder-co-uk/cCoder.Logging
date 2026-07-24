// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        LogEntry entity = CreateRandomLogEntry();

        logEntryProcessingServiceMock.Setup(expression: x => x.GetLogEntry(logEntryId: id))
            .Returns(value: entity);

        logEntryProcessingServiceMock.Setup(expression: x => x.DeleteLogEntryAsync(logEntryId: id))
            .Returns(value: ValueTask.CompletedTask);

        logEntryEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLogEntryDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteLogEntryAsync(logEntryId: id);

        // Then
        logEntryProcessingServiceMock.Verify(expression: x => x.GetLogEntry(logEntryId: id), times: Times.Once);
        logEntryProcessingServiceMock.Verify(expression: x => x.DeleteLogEntryAsync(logEntryId: id), times: Times.Once);
        logEntryEventProcessingServiceMock.Verify(expression: x => x.RaiseLogEntryDeleteEventAsync(entity: entity), times: Times.Once);
    }

}