// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogRetentionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCompleteWithoutFailureWhenRunLogRetentionAsyncIsCancelled()
    {
        // Given
        using CancellationTokenSource cancellationTokenSource = new();

        logEntryServiceMock
            .Setup(expression: service =>
                service.DeleteLogEntriesBeforeAsync(
                    cutoff: It.IsAny<DateTime>()))
            .ReturnsAsync(value: 0);

        cancellationTokenSource.CancelAfter(
            delay: TimeSpan.FromMilliseconds(value: 20));

        // When
        Task runTask = ProcessingService.RunLogRetentionAsync(
            cancellationToken: cancellationTokenSource.Token);

        // Then
        await runTask;

        logEntryServiceMock.Verify(
            expression: service =>
                service.DeleteLogEntriesBeforeAsync(
                    cutoff: It.IsAny<DateTime>()),
            times: Times.Once);
    }
}