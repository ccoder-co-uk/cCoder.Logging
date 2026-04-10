using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        LogEntry entity = CreateRandomLogEntry();
        logEntryProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        logEntryEventProcessingServiceMock
            .Setup(x => x.RaiseLogEntryAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        LogEntry result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        logEntryProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        logEntryEventProcessingServiceMock.Verify(x => x.RaiseLogEntryAddEventAsync(entity), Times.Once);
    }

}







