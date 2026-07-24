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
            .Setup(expression: processingService =>
                processingService.CaptureLogEntryAsync(
logEntryCaptureRequest: logEntryCaptureRequest))
            .ReturnsAsync(value: savedLogEntry);

        logEntryEventProcessingServiceMock
            .Setup(expression: processingService =>
                processingService.RaiseLogEntryAddEventAsync(
entity: savedLogEntry))
            .Returns(value: ValueTask.CompletedTask);

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
            .Setup(expression: processingService =>
                processingService.CaptureLogEntryAsync(
logEntryCaptureRequest: logEntryCaptureRequest))
            .ReturnsAsync(value: (LogEntry)null);

        // When

        await orchestrationService.CaptureLogEntryAsync(
            logEntryCaptureRequest: logEntryCaptureRequest);

        // Then
        logEntryCaptureProcessingServiceMock.VerifyAll();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}