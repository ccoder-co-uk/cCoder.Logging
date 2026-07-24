// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        DataLogEntry logEntry = CreateRandomDataLogEntry(id: 7);

        logEntryBrokerMock.Setup(x => x.GetAllLogEntries(false)).Returns(new[] { logEntry }.AsQueryable());

        // When
        LogEntry result = logEntryService.Get(7);

        // Then
        result.Should().BeEquivalentTo(logEntry);
        logEntryBrokerMock.Verify(x => x.GetAllLogEntries(false), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}