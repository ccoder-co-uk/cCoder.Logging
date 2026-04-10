using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        LogEntry entity = CreateRandomLogEntry();
        logEntryProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        LogEntry result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        logEntryProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







