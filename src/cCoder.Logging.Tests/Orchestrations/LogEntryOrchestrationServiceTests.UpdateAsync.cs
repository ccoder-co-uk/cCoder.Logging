// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        LogEntry entity = CreateRandomLogEntry();

        logEntryProcessingServiceMock.Setup(expression: x => x.UpdateLogEntryAsync(updatedLogEntry: entity))
            .ReturnsAsync(value: entity);

        logEntryEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLogEntryUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        LogEntry result = await orchestrationService.UpdateLogEntryAsync(updatedLogEntry: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        logEntryProcessingServiceMock.Verify(expression: x => x.UpdateLogEntryAsync(updatedLogEntry: entity), times: Times.Once);
        logEntryEventProcessingServiceMock.Verify(expression: x => x.RaiseLogEntryUpdateEventAsync(entity: entity), times: Times.Once);
    }

}