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
        logDataItemProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        logDataItemEventProcessingServiceMock
            .Setup(x => x.RaiseLogDataItemUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        LogDataItem result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        logDataItemProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        logDataItemEventProcessingServiceMock.Verify(x => x.RaiseLogDataItemUpdateEventAsync(entity), Times.Once);
    }

}