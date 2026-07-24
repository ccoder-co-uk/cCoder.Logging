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
        logEntryProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        logEntryEventProcessingServiceMock
            .Setup(x => x.RaiseLogEntryUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        LogEntry result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        logEntryProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        logEntryEventProcessingServiceMock.Verify(x => x.RaiseLogEntryUpdateEventAsync(entity), Times.Once);
    }

}