// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogRetentionOrchestrationServiceTests
{
    private readonly Mock<ILogEntryService> logEntryServiceMock = new(
        behavior: MockBehavior.Strict);

    private readonly LoggingConfiguration configuration = new()
    {
        StoreLogEntries = true,
        RetentionDays = 30,
        RetentionIntervalMinutes = 60
    };

    private LogEntryRetentionProcessingService ProcessingService =>
        new LogEntryRetentionProcessingService(
            logEntryService: logEntryServiceMock.Object,
            loggingConfiguration: configuration);
}