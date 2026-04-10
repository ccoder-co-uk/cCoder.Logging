using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;
using cCoder.Logging.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogEntryEventProcessingServiceTests
{
    private readonly Mock<ILogEntryEventService> logEntryEventServiceMock;
    private readonly LogEntryEventProcessingService service;

    public LogEntryEventProcessingServiceTests()
    {
        logEntryEventServiceMock = new Mock<ILogEntryEventService>(MockBehavior.Strict);
        service = new LogEntryEventProcessingService(logEntryEventServiceMock.Object);
    }

    private static LogEntry CreateRandomLogEntry() =>
        Builder<LogEntry>.CreateNew().Build();
}











