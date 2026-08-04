// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Exposures;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests.Exposures;

public partial class LogDataItemManagerTests
{
    [Fact]
    public async Task ShouldMapBatchOperationResults()
    {
        // Given

        var item = new LogDataItem { Id = 42 };
        LogDataItem[] input = [item];

        OperationResult<LogDataItem>[] operationResults =
        [
            new()
            {
                Success = false,
                Message = "validation failed",
                Item = item
            }
        ];

        var orchestrationServiceMock = new Mock<ILogDataItemOrchestrationService>();

        orchestrationServiceMock
            .Setup(expression:service => service.AddOrUpdateLogDataItemResultsAsync(logDataItems:input))
            .ReturnsAsync(value:operationResults);

        var manager = new LogDataItemManager(
            logDataItemOrchestrationService:orchestrationServiceMock.Object);

        // When

        IEnumerable<Result<LogDataItem>> actualResults =
            await manager.AddOrUpdateLogDataItemsAsync(logDataItems:input);

        // Then

        actualResults.Should()
            .ContainSingle().Which.Should()
            .BeEquivalentTo(
                expectation:new
                {
                    Success = false,
                    Message = "validation failed",
                    Item = item
                });

        orchestrationServiceMock.Verify(
            expression:service => service.AddOrUpdateLogDataItemResultsAsync(logDataItems:input),
            times:Times.Once);
    }
}