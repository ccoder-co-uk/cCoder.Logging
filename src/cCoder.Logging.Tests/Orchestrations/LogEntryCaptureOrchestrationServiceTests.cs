using cCoder.Data.Models.Logging;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryCaptureOrchestrationServiceTests
{
    private readonly Mock<ILogEntryProcessingService> logEntryProcessingServiceMock;
    private readonly Mock<ILogEntryEventProcessingService> logEntryEventProcessingServiceMock;
    private readonly Mock<ILogEntryStreamBroker> logEntryStreamBrokerMock;
    private readonly LoggingConfiguration configuration;
    private readonly LogEntryCaptureOrchestrationService orchestrationService;

    public LogEntryCaptureOrchestrationServiceTests()
    {
        logEntryProcessingServiceMock = new Mock<ILogEntryProcessingService>(MockBehavior.Strict);
        logEntryEventProcessingServiceMock = new Mock<ILogEntryEventProcessingService>(MockBehavior.Strict);
        logEntryStreamBrokerMock = new Mock<ILogEntryStreamBroker>(MockBehavior.Strict);
        configuration = new LoggingConfiguration
        {
            StoreLogEntries = true,
            StreamLogEntries = true,
            DefaultAppDomain = "localhost"
        };
        orchestrationService = new LogEntryCaptureOrchestrationService(
            logEntryProcessingServiceMock.Object,
            logEntryEventProcessingServiceMock.Object,
            logEntryStreamBrokerMock.Object,
            configuration);
    }

    private static LogEntryCaptureRequest CreateRequest() =>
        new()
        {
            Level = LogLevel.Information,
            CategoryName = "cCoder.Tests.LoggingBroker",
            Message = $"message-{Guid.NewGuid():N}",
            RequestDomain = "localhost"
        };
}
