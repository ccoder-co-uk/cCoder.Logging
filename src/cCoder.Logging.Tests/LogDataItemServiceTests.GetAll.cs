// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        DataLogDataItem logDataItem = CreateRandomDataLogDataItem();
        IQueryable<DataLogDataItem> logDataItems = new[] { logDataItem }.AsQueryable();

        logDataItemBrokerMock.Setup(x => x.GetAllLogDataItems(false)).Returns(logDataItems);

        // When
        IQueryable<LogDataItem> result = logDataItemService.GetAll();

        // Then
        result.Should().BeEquivalentTo(logDataItems, options => options.ExcludingMissingMembers());
        logDataItemBrokerMock.Verify(x => x.GetAllLogDataItems(false), Times.Once);
        logDataItemBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataLogDataItem>()), Times.AtMostOnce());
        logDataItemBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}