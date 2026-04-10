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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        LogEntry logEntry = CreateRandomLogEntry();
        DataLogEntry storedLogEntry = CreateRandomDataLogEntry(id: logEntry.Id);

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogEntry_update"));
        logEntryBrokerMock
            .Setup(x => x.UpdateLogEntryAsync(It.IsAny<DataLogEntry>()))
            .ReturnsAsync(storedLogEntry);

        // When
        LogEntry result = await logEntryService.UpdateAsync(logEntry);

        // Then
        result.Should().BeEquivalentTo(storedLogEntry);
        logEntryBrokerMock.Verify(x => x.UpdateLogEntryAsync(It.IsAny<DataLogEntry>()), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        LogEntry logEntry = CreateRandomLogEntry();

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogEntry_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logEntryService.UpdateAsync(logEntry);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}






