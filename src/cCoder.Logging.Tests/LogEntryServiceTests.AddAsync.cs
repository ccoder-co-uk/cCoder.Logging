// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        LogEntry logEntry = CreateRandomLogEntry(id: 0);
        DataLogEntry storedLogEntry = CreateRandomDataLogEntry(id: 0);

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogEntry_create"));
        logEntryBrokerMock
            .Setup(x => x.AddLogEntryAsync(It.IsAny<DataLogEntry>()))
            .ReturnsAsync(storedLogEntry);

        // When
        LogEntry result = await logEntryService.AddAsync(logEntry);

        // Then
        result.Should().BeEquivalentTo(storedLogEntry);
        logEntryBrokerMock.Verify(x => x.AddLogEntryAsync(It.IsAny<DataLogEntry>()), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        LogEntry logEntry = CreateRandomLogEntry(id: 0);

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogEntry_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logEntryService.AddAsync(logEntry);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}