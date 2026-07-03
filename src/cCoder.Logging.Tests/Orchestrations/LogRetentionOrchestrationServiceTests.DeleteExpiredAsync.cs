using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogRetentionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteEntriesOlderThanRetentionPeriodWhenDeleteExpiredAsync()
    {
        // Given
        DateTime before = DateTime.UtcNow.AddDays(-30).AddSeconds(-5);
        DateTime capturedCutoff = default;
        logEntryProcessingServiceMock
            .Setup(service => service.DeleteEntriesBeforeAsync(It.IsAny<DateTime>()))
            .Callback<DateTime>(cutoff => capturedCutoff = cutoff)
            .ReturnsAsync(3);

        // When
        int result = await orchestrationService.DeleteExpiredAsync();
        DateTime after = DateTime.UtcNow.AddDays(-30).AddSeconds(5);

        // Then
        Assert.Equal(3, result);
        Assert.InRange(capturedCutoff, before, after);
        logEntryProcessingServiceMock.Verify(service => service.DeleteEntriesBeforeAsync(It.IsAny<DateTime>()), Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotDeleteEntriesWhenStorageIsDisabled()
    {
        // Given
        configuration.StoreLogEntries = false;

        // When
        int result = await orchestrationService.DeleteExpiredAsync();

        // Then
        Assert.Equal(0, result);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
    }
}
