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
        logDataItemProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        logDataItemEventProcessingServiceMock
            .Setup(x => x.RaiseLogDataItemAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        LogDataItem result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        logDataItemProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        logDataItemEventProcessingServiceMock.Verify(x => x.RaiseLogDataItemAddEventAsync(entity), Times.Once);
    }

}







