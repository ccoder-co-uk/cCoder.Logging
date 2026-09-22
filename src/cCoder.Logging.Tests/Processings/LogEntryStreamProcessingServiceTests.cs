// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Processings;

public sealed partial class LogEntryStreamProcessingServiceTests
{
    [Fact]
    public async Task ShouldStreamLogEntryWhenConfiguredAsync()
    {
        // Given
        LogEntryCaptureRequest request = new()
        {
            RequestDomain = "example.test",
            Level = LogLevel.Warning,
            Message = "message"
        };

        Mock<ILogEntryStreamService> serviceMock = new(
            behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.GetDefaultAppDomain())
            .Returns(value: null);

        serviceMock
            .Setup(expression: service => service.GetDefaultAppId())
            .Returns(value: null);

        serviceMock
            .Setup(expression: service => service.ShouldStreamLogEntries())
            .Returns(value: true);

        serviceMock
            .Setup(expression: service => service.StreamLogEntryAsync(
                thread: request.RequestDomain,
                level: "warning",
                message: request.Message))
            .Returns(value: ValueTask.CompletedTask);

        LogEntryStreamProcessingService processingService = new(
            logEntryStreamService: serviceMock.Object);

        // When
        await processingService.StreamLogEntryCaptureRequestAsync(
            logEntryCaptureRequest: request);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotStreamLogEntryWhenThreadIsUnavailableAsync()
    {
        // Given
        LogEntryCaptureRequest request = new()
        {
            Level = LogLevel.Information,
            Message = "message"
        };

        Mock<ILogEntryStreamService> serviceMock = new(
            behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.GetDefaultAppDomain())
            .Returns(value: null);

        serviceMock
            .Setup(expression: service => service.GetDefaultAppId())
            .Returns(value: null);

        serviceMock
            .Setup(expression: service => service.ShouldStreamLogEntries())
            .Returns(value: true);

        LogEntryStreamProcessingService processingService = new(
            logEntryStreamService: serviceMock.Object);

        // When
        await processingService.StreamLogEntryCaptureRequestAsync(
            logEntryCaptureRequest: request);

        // Then
        serviceMock.VerifyAll();
    }
}