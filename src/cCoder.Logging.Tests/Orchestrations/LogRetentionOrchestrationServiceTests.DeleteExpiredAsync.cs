// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        logEntryServiceMock
            .Setup(service => service.DeleteLogEntriesBeforeAsync(
                It.IsAny<DateTime>()))
            .Callback<DateTime>(cutoff => capturedCutoff = cutoff)
            .ReturnsAsync(3);

        // When
        int result = await processingService.DeleteExpiredLogEntriesAsync();
        DateTime after = DateTime.UtcNow.AddDays(-30).AddSeconds(5);

        // Then
        Assert.Equal(3, result);
        Assert.InRange(capturedCutoff, before, after);
        logEntryServiceMock.Verify(service =>
            service.DeleteLogEntriesBeforeAsync(
                It.IsAny<DateTime>()),
            Times.Once);

        logEntryServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotDeleteEntriesWhenStorageIsDisabled()
    {
        // Given
        configuration.StoreLogEntries = false;

        // When
        int result = await processingService.DeleteExpiredLogEntriesAsync();

        // Then
        Assert.Equal(0, result);
        logEntryServiceMock.VerifyNoOtherCalls();
    }
}
