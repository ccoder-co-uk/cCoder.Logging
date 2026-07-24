// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        LogDataItem logDataItem = CreateRandomLogDataItem(id: 0);
        DataLogDataItem storedLogDataItem = CreateRandomDataLogDataItem(id: 0);

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "LogDataItem_create"));

        logDataItemBrokerMock
            .Setup(x => x.AddLogDataItemAsync(It.IsAny<DataLogDataItem>()))
            .ReturnsAsync(storedLogDataItem);

        // When
        LogDataItem result = await logDataItemService.AddAsync(logDataItem);

        // Then
        result.Should().BeEquivalentTo(
            storedLogDataItem,
            options => options.ExcludingMissingMembers());
        logDataItemBrokerMock.Verify(x => x.AddLogDataItemAsync(It.IsAny<DataLogDataItem>()), Times.Once);
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        LogDataItem logDataItem = CreateRandomLogDataItem(id: 0);

        logDataItemBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataLogDataItem>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "LogDataItem_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await logDataItemService.AddAsync(logDataItem);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "LogDataItem_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}