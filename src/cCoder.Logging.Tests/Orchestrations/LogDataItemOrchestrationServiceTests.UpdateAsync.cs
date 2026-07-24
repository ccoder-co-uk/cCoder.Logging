// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogDataItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        LogDataItem entity = CreateRandomLogDataItem();

        logDataItemProcessingServiceMock.Setup(expression: x => x.UpdateLogDataItemAsync(updatedLogDataItem: entity))
            .ReturnsAsync(value: entity);

        logDataItemEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLogDataItemUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        LogDataItem result = await orchestrationService.UpdateLogDataItemAsync(updatedLogDataItem: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        logDataItemProcessingServiceMock.Verify(expression: x => x.UpdateLogDataItemAsync(updatedLogDataItem: entity), times: Times.Once);
        logDataItemEventProcessingServiceMock.Verify(expression: x => x.RaiseLogDataItemUpdateEventAsync(entity: entity), times: Times.Once);
    }

}