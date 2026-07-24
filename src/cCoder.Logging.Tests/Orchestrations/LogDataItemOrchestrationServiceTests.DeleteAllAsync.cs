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
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        LogDataItem[] entities = [CreateRandomLogDataItem()];

        logDataItemProcessingServiceMock.Setup(expression: x => x.DeleteAllLogDataItemAsync(deletedLogDataItems: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllLogDataItemAsync(deletedLogDataItems: entities);

        // Then
        logDataItemProcessingServiceMock.Verify(expression: x => x.DeleteAllLogDataItemAsync(deletedLogDataItems: entities), times: Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}