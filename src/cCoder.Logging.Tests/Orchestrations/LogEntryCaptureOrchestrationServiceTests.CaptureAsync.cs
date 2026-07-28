// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryCaptureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventWhenCaptureLogEntryCaptureRequestAsync()
    {
        // Given
        LogEntryCaptureRequest logEntryCaptureRequest = CreateRequest();
        LogEntry savedLogEntry = CreateLogEntry();

        logEntryCaptureProcessingServiceMock
            .Setup(expression: processingService =>
                processingService.CaptureLogEntryCaptureOperationAsync(
                    operation: It.Is<LogEntryCaptureOperation>(
                        match: operation =>
                            operation.Request == logEntryCaptureRequest)))
            .ReturnsAsync(
                value: new LogEntryCaptureOperation
                {
                    Request = logEntryCaptureRequest,
                    Result = savedLogEntry
                });

        logEntryEventProcessingServiceMock
            .Setup(expression: processingService =>
                processingService.RaiseLogEntryAddEventAsync(
entity: savedLogEntry))
            .Returns(value: ValueTask.CompletedTask);

        // When

        await orchestrationService.CaptureLogEntryCaptureRequestAsync(
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
                processingService.CaptureLogEntryCaptureOperationAsync(
                    operation: It.Is<LogEntryCaptureOperation>(
                        match: operation =>
                            operation.Request == logEntryCaptureRequest)))
            .ReturnsAsync(
                value: new LogEntryCaptureOperation
                {
                    Request = logEntryCaptureRequest
                });

        // When

        await orchestrationService.CaptureLogEntryCaptureRequestAsync(
            logEntryCaptureRequest: logEntryCaptureRequest);

        // Then
        logEntryCaptureProcessingServiceMock.VerifyAll();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}