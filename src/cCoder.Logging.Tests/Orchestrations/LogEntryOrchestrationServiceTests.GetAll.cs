using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<LogEntry> entities = new[] { CreateRandomLogEntry() }.AsQueryable();
        logEntryProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<LogEntry> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        logEntryProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







