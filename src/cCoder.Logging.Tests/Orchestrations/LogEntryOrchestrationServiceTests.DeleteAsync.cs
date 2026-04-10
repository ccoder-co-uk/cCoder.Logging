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
        logEntryProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        logEntryProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        logEntryEventProcessingServiceMock
            .Setup(x => x.RaiseLogEntryDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        logEntryProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        logEntryProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        logEntryEventProcessingServiceMock.Verify(x => x.RaiseLogEntryDeleteEventAsync(entity), Times.Once);
    }

}







