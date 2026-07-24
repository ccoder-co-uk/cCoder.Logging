// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging;

public partial class LogDataItemServiceTests
{
    [Fact]
    public void ShouldReturnLogDataItemWhenGetLogDataItem()
    {
        // Given
        LogDataItem expectedLogDataItem = CreateRandomLogDataItem();

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAllLogDataItems())
            .Returns(value: new[] { expectedLogDataItem }.AsQueryable());

        // When

        LogDataItem actualLogDataItem =
            logDataItemService.GetLogDataItem(
                logDataItemId: expectedLogDataItem.Id);

        // Then

        actualLogDataItem.Id.Should()
            .Be(expected: expectedLogDataItem.Id);

        logDataItemBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldReturnLogDataItemsWhenGetAllLogDataItems()
    {
        // Given
        IQueryable<LogDataItem> expectedLogDataItems =
            new[] { CreateRandomLogDataItem() }.AsQueryable();

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAllLogDataItems())
            .Returns(value: expectedLogDataItems);

        // When

        IQueryable<LogDataItem> actualLogDataItems =
            logDataItemService.GetAllLogDataItems();

        // Then

        actualLogDataItems.Should()
            .BeSameAs(expected: expectedLogDataItems);

        logDataItemBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldAddLogDataItemWhenAddLogDataItemAsync()
    {
        // Given
        LogDataItem newLogDataItem = CreateRandomLogDataItem(id: 0);
        const int AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogDataItem_create",
            appId: AppId);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAppIdByLogDataItem(
logDataItem: newLogDataItem))
            .Returns(value: AppId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.InsertLogDataItemAsync(
newLogDataItem: It.IsAny<LogDataItem>()))
            .ReturnsAsync(valueFunction: (LogDataItem logDataItem) =>
            {
                logDataItem.Id = 42;
                return logDataItem;
            });

        // When

        LogDataItem savedLogDataItem =
            await logDataItemService.AddLogDataItemAsync(
                newLogDataItem: newLogDataItem);

        // Then

        savedLogDataItem.Should()
            .BeSameAs(expected: newLogDataItem);

        savedLogDataItem.Id.Should()
            .Be(expected: 42);

        authorizationBrokerMock.VerifyAll();
        logDataItemBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldUpdateLogDataItemWhenUpdateLogDataItemAsync()
    {
        // Given
        LogDataItem updatedLogDataItem = CreateRandomLogDataItem();
        const int AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogDataItem_update",
            appId: AppId);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAppIdByLogDataItem(
logDataItem: updatedLogDataItem))
            .Returns(value: AppId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.UpdateLogDataItemAsync(
updatedLogDataItem: It.IsAny<LogDataItem>()))
            .ReturnsAsync(valueFunction: (LogDataItem logDataItem) => logDataItem);

        // When

        LogDataItem savedLogDataItem =
            await logDataItemService.UpdateLogDataItemAsync(
                updatedLogDataItem: updatedLogDataItem);

        // Then

        savedLogDataItem.Should()
            .BeSameAs(expected: updatedLogDataItem);

        authorizationBrokerMock.VerifyAll();
        logDataItemBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteLogDataItemWhenDeleteLogDataItemAsync()
    {
        // Given
        LogDataItem deletedLogDataItem = CreateRandomLogDataItem();
        const int AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogDataItem_delete",
            appId: AppId);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAllLogDataItems())
            .Returns(value: new[] { deletedLogDataItem }.AsQueryable());

        logDataItemBrokerMock
            .Setup(expression: broker => broker.SelectAppIdByLogDataItem(
logDataItem: It.IsAny<LogDataItem>()))
            .Returns(value: AppId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logDataItemBrokerMock
            .Setup(expression: broker => broker.DeleteLogDataItemAsync(
deletedLogDataItem: It.IsAny<LogDataItem>()))
            .ReturnsAsync(value: 1);

        // When

        await logDataItemService.DeleteLogDataItemAsync(
            logDataItemId: deletedLogDataItem.Id);

        // Then
        authorizationBrokerMock.VerifyAll();
        logDataItemBrokerMock.VerifyAll();
    }
}