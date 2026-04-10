using System.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataLogEntry = cCoder.Data.Models.Logging.LogEntry;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogEntryServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        DataLogEntry logEntry = CreateRandomDataLogEntry(id: 9);

        logEntryBrokerMock.Setup(x => x.GetAllLogEntries(false)).Returns(new[] { logEntry }.AsQueryable());

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogEntry_delete"));
        logEntryBrokerMock.Setup(x => x.DeleteLogEntryAsync(It.IsAny<DataLogEntry>())).ReturnsAsync(1);

        // When
        await logEntryService.DeleteAsync(9);

        // Then
        logEntryBrokerMock.Verify(x => x.GetAllLogEntries(false), Times.Once);
        logEntryBrokerMock.Verify(x => x.DeleteLogEntryAsync(It.IsAny<DataLogEntry>()), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        DataLogEntry logEntry = CreateRandomDataLogEntry(id: 9);

        logEntryBrokerMock.Setup(x => x.GetAllLogEntries(false)).Returns(new[] { logEntry }.AsQueryable());

        logEntryBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogEntry>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogEntry_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logEntryService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logEntryBrokerMock.Verify(x => x.GetAllLogEntries(false), Times.Once);
        logEntryBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogEntry>()), Times.AtMostOnce());
        logEntryBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogEntry_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}






