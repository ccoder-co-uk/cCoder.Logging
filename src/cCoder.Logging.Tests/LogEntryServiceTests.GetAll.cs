using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;
using DataLogEntry = cCoder.Data.Models.Logging.LogEntry;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogEntryServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        DataLogEntry logEntry = CreateRandomDataLogEntry();
        IQueryable<DataLogEntry> logEntries = new[] { logEntry }.AsQueryable();

        logEntryBrokerMock.Setup(x => x.GetAllLogEntries(false)).Returns(logEntries);

        // When
        IQueryable<LogEntry> result = logEntryService.GetAll();

        // Then
        result.Should().BeEquivalentTo(logEntries);
        logEntryBrokerMock.Verify(x => x.GetAllLogEntries(false), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







