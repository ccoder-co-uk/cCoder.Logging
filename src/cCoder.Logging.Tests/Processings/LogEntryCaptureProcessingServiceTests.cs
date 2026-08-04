// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Processings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogEntryCaptureProcessingServiceTests
{
    [Fact]
    public async Task ShouldPersistRequestAnalyticsContextAsync()
    {
        // Given
        const int AppId = 17;
        const string TenantId = "tenant-42";

        Mock<ILogEntryService> logEntryServiceMock = new(
            behavior: MockBehavior.Strict);

        Mock<ILogEntryStreamBroker> logEntryStreamBrokerMock = new(
            behavior: MockBehavior.Strict);

        LoggingConfiguration loggingConfiguration = new()
        {
            DefaultAppId = AppId,
            StoreLogEntries = true,
            StreamLogEntries = false
        };

        LogEntryCaptureRequest request = new()
        {
            CategoryName = "RequestAnalytics",
            Level = LogLevel.Information,
            Message = "Request completed",
            RequestDomain = "example.test",
            Url = "https://example.test/orders?status=open",
            UserId = "user-7",
            SessionId = "session-9"
        };

        logEntryServiceMock
            .Setup(expression: service =>
                service.ResolveTenantId(appId: AppId))
            .Returns(value: TenantId);

        logEntryServiceMock
            .Setup(expression: service =>
                service.AddSystemLogEntryAsync(
                    newLogEntry: It.Is<LogEntry>(match: entry =>
                        entry.AppId == AppId
                        && entry.Data.Any(predicate: item =>
                            item.Name == "Url"
                            && item.Value == request.Url)
                        && entry.Data.Any(predicate: item =>
                            item.Name == "UserId"
                            && item.Value == request.UserId)
                        && entry.Data.Any(predicate: item =>
                            item.Name == "SessionId"
                            && item.Value == request.SessionId)
                        && entry.Data.Any(predicate: item =>
                            item.Name == "TenantId"
                            && item.Value == TenantId)
                        && entry.Data.Any(predicate: item =>
                            item.Name == "AppId"
                            && item.Value == AppId.ToString()))))
            .ReturnsAsync(value: new LogEntry { Id = 1 });

        LogEntryCaptureProcessingService processingService = new(
            logEntryService: logEntryServiceMock.Object,
            logEntryStreamBroker: logEntryStreamBrokerMock.Object,
            loggingConfiguration: loggingConfiguration);

        // When
        LogEntryCaptureOperation result =
            await processingService.CaptureLogEntryCaptureOperationAsync(
                operation: new LogEntryCaptureOperation
                {
                    Request = request
                });

        // Then
        result.Result
            .Should()
            .NotBeNull();

        logEntryServiceMock.VerifyAll();
        logEntryStreamBrokerMock.VerifyNoOtherCalls();
    }

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