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
        logDataItemProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        logDataItemProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        logDataItemEventProcessingServiceMock
            .Setup(x => x.RaiseLogDataItemDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        logDataItemProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        logDataItemProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        logDataItemEventProcessingServiceMock.Verify(x => x.RaiseLogDataItemDeleteEventAsync(entity), Times.Once);
    }

}







