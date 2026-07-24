// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    private readonly Mock<ILogEntryProcessingService> logEntryProcessingServiceMock;
    private readonly Mock<ILogEntryEventProcessingService> logEntryEventProcessingServiceMock;
    private readonly LogEntryOrchestrationService orchestrationService;

    public LogEntryOrchestrationServiceTests()
    {
        logEntryProcessingServiceMock = new Mock<ILogEntryProcessingService>(behavior: MockBehavior.Strict);
        logEntryEventProcessingServiceMock = new Mock<ILogEntryEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new LogEntryOrchestrationService(
logEntryProcessingService: logEntryProcessingServiceMock.Object,
logEntryEventProcessingService: logEntryEventProcessingServiceMock.Object
        );
    }

    private static LogEntry CreateRandomLogEntry() =>
        Builder<LogEntry>.CreateNew()
        .Build();
}