// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryCaptureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventWhenCaptureLogEntryAsync()
    {
        // Given
        LogEntryCaptureRequest logEntryCaptureRequest = CreateRequest();
        LogEntry savedLogEntry = CreateLogEntry();

        logEntryCaptureProcessingServiceMock
            .Setup(processingService =>
                processingService.CaptureLogEntryAsync(
                    logEntryCaptureRequest))
            .ReturnsAsync(savedLogEntry);

        logEntryEventProcessingServiceMock
            .Setup(processingService =>
                processingService.RaiseLogEntryAddEventAsync(
                    savedLogEntry))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.CaptureLogEntryAsync(
            logEntryCaptureRequest: logEntryCaptureRequest);

        // Then
        logEntryCaptureProcessingServiceMock.VerifyAll();
        logEntryEventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotRaiseEventWhenCaptureReturnsNoLogEntryAsync()
    {
        // Given
        LogEntryCaptureRequest logEntryCaptureRequest = CreateRequest();

        logEntryCaptureProcessingServiceMock
            .Setup(processingService =>
                processingService.CaptureLogEntryAsync(
                    logEntryCaptureRequest))
            .ReturnsAsync((LogEntry)null);

        // When
        await orchestrationService.CaptureLogEntryAsync(
            logEntryCaptureRequest: logEntryCaptureRequest);

        // Then
        logEntryCaptureProcessingServiceMock.VerifyAll();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}
