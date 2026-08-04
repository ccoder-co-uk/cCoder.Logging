// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests.Processings;

public partial class LogEntryProcessingServiceTests
{
    [Fact]
    public async Task ShouldContinueBatchAfterIndividualFailure()
    {
        // Given

        var failedEntry = new LogEntry { Message = "failed" };
        var savedEntry = new LogEntry { Id = 42, Message = "saved" };
        var successfulEntry = new LogEntry { Message = "saved" };
        LogEntry[] entries = [failedEntry, successfulEntry];
        var logEntryServiceMock = new Mock<ILogEntryService>(behavior:MockBehavior.Strict);

        logEntryServiceMock
            .Setup(expression:service => service.AddLogEntryAsync(
                newLogEntry:It.Is<LogEntry>(match:entry => entry.Message == failedEntry.Message)))
            .ThrowsAsync(exception:new InvalidOperationException(message:"storage unavailable"));

        logEntryServiceMock
            .Setup(expression:service => service.AddLogEntryAsync(
                newLogEntry:It.Is<LogEntry>(match:entry => entry.Message == successfulEntry.Message)))
            .ReturnsAsync(value:savedEntry);

        var processingService = new LogEntryProcessingService(
            logEntryService:logEntryServiceMock.Object);

        // When

        IEnumerable<OperationResult<LogEntry>> actualResults =
            await processingService.AddOrUpdateLogEntryResultsAsync(
                logEntries:entries);

        // Then

        OperationResult<LogEntry>[] results = actualResults.ToArray();

        results.Should()
            .HaveCount(expected:2);

        results[0].Should()
            .BeEquivalentTo(
                expectation:new
                {
                    Success = false,
                    Message = "storage unavailable",
                    Item = failedEntry
                });

        results[1].Should()
            .BeEquivalentTo(
                expectation:new
                {
                    Success = true,
                    Message = "Added Successfully",
                    Item = savedEntry
                });

        logEntryServiceMock.VerifyAll();
    }
}