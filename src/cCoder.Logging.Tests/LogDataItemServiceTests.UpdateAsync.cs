using System.Security;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogDataItemServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        LogDataItem logDataItem = CreateRandomLogDataItem();
        DataLogDataItem storedLogDataItem = CreateRandomDataLogDataItem(
            id: logDataItem.Id,
            logEntryId: logDataItem.LogEntryId);

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogDataItem_update"));

        logDataItemBrokerMock
            .Setup(x => x.UpdateLogDataItemAsync(It.IsAny<DataLogDataItem>()))
            .ReturnsAsync(storedLogDataItem);

        // When
        LogDataItem result = await logDataItemService.UpdateAsync(logDataItem);

        // Then
        result.Should().BeEquivalentTo(
            storedLogDataItem,
            options => options.ExcludingMissingMembers());
        logDataItemBrokerMock.Verify(x => x.UpdateLogDataItemAsync(It.IsAny<DataLogDataItem>()), Times.Once);
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        LogDataItem logDataItem = CreateRandomLogDataItem();

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogDataItem_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logDataItemService.UpdateAsync(logDataItem);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}






