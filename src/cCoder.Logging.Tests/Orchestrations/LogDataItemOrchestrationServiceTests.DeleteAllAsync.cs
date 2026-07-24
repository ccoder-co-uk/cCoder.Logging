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
        logDataItemProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        logDataItemProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}