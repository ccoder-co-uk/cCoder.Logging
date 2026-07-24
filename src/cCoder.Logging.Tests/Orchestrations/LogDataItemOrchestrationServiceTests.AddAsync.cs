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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        LogDataItem entity = CreateRandomLogDataItem();

        logDataItemProcessingServiceMock.Setup(expression: x => x.AddLogDataItemAsync(newLogDataItem: entity))
            .ReturnsAsync(value: entity);

        logDataItemEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLogDataItemAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        LogDataItem result = await orchestrationService.AddLogDataItemAsync(newLogDataItem: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        logDataItemProcessingServiceMock.Verify(expression: x => x.AddLogDataItemAsync(newLogDataItem: entity), times: Times.Once);
        logDataItemEventProcessingServiceMock.Verify(expression: x => x.RaiseLogDataItemAddEventAsync(entity: entity), times: Times.Once);
    }

}