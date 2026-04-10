using System.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogDataItemServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        DataLogDataItem logDataItem = CreateRandomDataLogDataItem(id: 9);

        logDataItemBrokerMock.Setup(x => x.GetAllLogDataItems(false)).Returns(new[] { logDataItem }.AsQueryable());

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogDataItem_delete"));
        logDataItemBrokerMock
            .Setup(x => x.DeleteLogDataItemAsync(It.IsAny<DataLogDataItem>()))
            .ReturnsAsync(1);

        // When
        await logDataItemService.DeleteAsync(9);

        // Then
        logDataItemBrokerMock.Verify(x => x.GetAllLogDataItems(false), Times.Once);
        logDataItemBrokerMock.Verify(x => x.DeleteLogDataItemAsync(It.IsAny<DataLogDataItem>()), Times.Once);
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        DataLogDataItem logDataItem = CreateRandomDataLogDataItem(id: 9);

        logDataItemBrokerMock.Setup(x => x.GetAllLogDataItems(false)).Returns(new[] { logDataItem }.AsQueryable());

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogDataItem_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logDataItemService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logDataItemBrokerMock.Verify(x => x.GetAllLogDataItems(false), Times.Once);
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}






