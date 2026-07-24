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
        DateTime before = DateTime.UtcNow.AddDays(value: -30)
            .AddSeconds(value: -5);

        DateTime capturedCutoff = default;

        logEntryServiceMock
            .Setup(expression: service => service.DeleteLogEntriesBeforeAsync(
cutoff: It.IsAny<DateTime>()))
            .Callback<DateTime>(action: cutoff => capturedCutoff = cutoff)
            .ReturnsAsync(value: 3);

        // When
        int result = await processingService.DeleteExpiredLogEntriesAsync();

        DateTime after = DateTime.UtcNow.AddDays(value: -30)
            .AddSeconds(value: 5);

        // Then
        Assert.Equal(expected: 3, actual: result);
        Assert.InRange(actual: capturedCutoff, low: before, high: after);

        logEntryServiceMock.Verify(expression: service =>
            service.DeleteLogEntriesBeforeAsync(
cutoff: It.IsAny<DateTime>()),
times: Times.Once);

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
        Assert.Equal(expected: 0, actual: result);
        logEntryServiceMock.VerifyNoOtherCalls();
    }
}