using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services;
using FizzWare.NBuilder;
using Moq;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;
using DataLogEntry = cCoder.Data.Models.Logging.LogEntry;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogEntryServiceTests
{
    private readonly Mock<ILogEntryBroker> logEntryBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly LogEntryService logEntryService;

    public LogEntryServiceTests()
    {
        logEntryBrokerMock = new Mock<ILogEntryBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        logEntryService = new LogEntryService(
            logEntryBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static LogEntry CreateRandomLogEntry(int id = 42)
    {
        LogEntry logEntry = Builder<LogEntry>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.Level = (int)cCoder.Logging.Models.LoggingLevel.Info)
            .With(x => x.Message = $"Message-{Guid.NewGuid():N}")
            .With(x => x.AppName = $"App-{Guid.NewGuid():N}")
            .With(x => x.TypeName = $"Type-{Guid.NewGuid():N}")
            .With(x => x.Date = DateTime.UtcNow)
            .With(x => x.Data = Array.Empty<LogDataItem>())
            .Build();

        return logEntry;
    }

    private static DataLogEntry CreateRandomDataLogEntry(int id = 42)
    {
        DataLogEntry logEntry = Builder<DataLogEntry>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.Level = (int)cCoder.Logging.Models.LoggingLevel.Info)
            .With(x => x.Message = $"Message-{Guid.NewGuid():N}")
            .With(x => x.AppName = $"App-{Guid.NewGuid():N}")
            .With(x => x.TypeName = $"Type-{Guid.NewGuid():N}")
            .With(x => x.Date = DateTime.UtcNow)
            .With(x => x.Data = Array.Empty<DataLogDataItem>())
            .Build();

        return logEntry;
    }
}












