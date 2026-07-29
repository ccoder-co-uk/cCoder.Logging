// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogEntryCaptureProcessingServiceTests
{
    [Fact]
    public async Task ShouldNotStreamLogEntryWhenThreadIsUnavailable()
    {
        // Given
        Mock<ILogEntryService> logEntryServiceMock = new(
            behavior: MockBehavior.Strict);

        Mock<ILogEntryStreamBroker> logEntryStreamBrokerMock = new(
            behavior: MockBehavior.Strict);

        LoggingConfiguration loggingConfiguration = new()
        {
            StreamLogEntries = true,
            StoreLogEntries = false
        };

        LogEntryCaptureRequest logEntryCaptureRequest = new()
        {
            CategoryName = "HostedServices",
            Level = LogLevel.Information,
            Message = "Application started"
        };

        LogEntryCaptureProcessingService processingService = new(
            logEntryService: logEntryServiceMock.Object,
            logEntryStreamBroker: logEntryStreamBrokerMock.Object,
            loggingConfiguration: loggingConfiguration);

        // When
        await processingService.CaptureLogEntryCaptureOperationAsync(
            operation: new LogEntryCaptureOperation
            {
                Request = logEntryCaptureRequest
            });

        // Then
        logEntryStreamBrokerMock.VerifyNoOtherCalls();
        logEntryServiceMock.VerifyNoOtherCalls();
    }
}