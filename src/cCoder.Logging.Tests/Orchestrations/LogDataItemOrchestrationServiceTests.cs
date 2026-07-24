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

public partial class LogDataItemOrchestrationServiceTests
{
    private readonly Mock<ILogDataItemProcessingService> logDataItemProcessingServiceMock;
    private readonly Mock<ILogDataItemEventProcessingService> logDataItemEventProcessingServiceMock;
    private readonly LogDataItemOrchestrationService orchestrationService;

    public LogDataItemOrchestrationServiceTests()
    {
        logDataItemProcessingServiceMock = new Mock<ILogDataItemProcessingService>(behavior: MockBehavior.Strict);
        logDataItemEventProcessingServiceMock = new Mock<ILogDataItemEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new LogDataItemOrchestrationService(
logDataItemProcessingService: logDataItemProcessingServiceMock.Object,
logDataItemEventProcessingService: logDataItemEventProcessingServiceMock.Object
        );
    }

    private static LogDataItem CreateRandomLogDataItem() =>
        Builder<LogDataItem>.CreateNew()
        .Build();
}