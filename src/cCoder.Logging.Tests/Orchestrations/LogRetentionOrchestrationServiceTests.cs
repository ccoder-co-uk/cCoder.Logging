// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogRetentionOrchestrationServiceTests
{
    private readonly Mock<ILogEntryProcessingService> logEntryProcessingServiceMock;
    private readonly LoggingConfiguration configuration;
    private readonly LogRetentionOrchestrationService orchestrationService;

    public LogRetentionOrchestrationServiceTests()
    {
        logEntryProcessingServiceMock = new Mock<ILogEntryProcessingService>(MockBehavior.Strict);
        configuration = new LoggingConfiguration
        {
            StoreLogEntries = true,
            RetentionDays = 30,
            RetentionIntervalMinutes = 60
        };
        orchestrationService = new LogRetentionOrchestrationService(
            logEntryProcessingServiceMock.Object,
            configuration);
    }
}