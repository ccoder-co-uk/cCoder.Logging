// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryCaptureOrchestrationServiceTests
{
    private readonly Mock<ILogEntryCaptureProcessingService>
        logEntryCaptureProcessingServiceMock;

    private readonly Mock<ILogEntryEventProcessingService>
        logEntryEventProcessingServiceMock;

    private readonly LogEntryCaptureOrchestrationService orchestrationService;

    public LogEntryCaptureOrchestrationServiceTests()
    {
        logEntryCaptureProcessingServiceMock =
            new Mock<ILogEntryCaptureProcessingService>(
                behavior: MockBehavior.Strict);

        logEntryEventProcessingServiceMock =
            new Mock<ILogEntryEventProcessingService>(
                behavior: MockBehavior.Strict);

        orchestrationService = new LogEntryCaptureOrchestrationService(
            logEntryCaptureProcessingService:
                logEntryCaptureProcessingServiceMock.Object,
            logEntryEventProcessingService:
                logEntryEventProcessingServiceMock.Object);
    }

    private static LogEntryCaptureRequest CreateRequest() =>
        new()
        {
            Level = LogLevel.Information,
            CategoryName = "cCoder.Tests.LoggingBroker",
            Message = $"message-{Guid.NewGuid():N}",
            RequestDomain = "localhost"
        };

    private static LogEntry CreateLogEntry() =>
        new()
        {
            Id = 7,
            AppId = 3,
            AppName = "localhost",
            Message = $"message-{Guid.NewGuid():N}"
        };
}