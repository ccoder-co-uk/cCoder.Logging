// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogDataItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        LogDataItem entity = CreateRandomLogDataItem();

        logDataItemProcessingServiceMock.Setup(expression: x => x.GetLogDataItem(logDataItemId: id))
            .Returns(value: entity);

        logDataItemProcessingServiceMock.Setup(expression: x => x.DeleteLogDataItemAsync(logDataItemId: id))
            .Returns(value: ValueTask.CompletedTask);

        logDataItemEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLogDataItemDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteLogDataItemAsync(logDataItemId: id);

        // Then
        logDataItemProcessingServiceMock.Verify(expression: x => x.GetLogDataItem(logDataItemId: id), times: Times.Once);
        logDataItemProcessingServiceMock.Verify(expression: x => x.DeleteLogDataItemAsync(logDataItemId: id), times: Times.Once);
        logDataItemEventProcessingServiceMock.Verify(expression: x => x.RaiseLogDataItemDeleteEventAsync(entity: entity), times: Times.Once);
    }

}