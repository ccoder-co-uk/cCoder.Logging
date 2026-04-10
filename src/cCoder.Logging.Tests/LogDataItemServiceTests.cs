using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services;
using FizzWare.NBuilder;
using Moq;
using DataLogDataItem = cCoder.Data.Models.Logging.LogDataItem;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Logging;

public partial class LogDataItemServiceTests
{
    private readonly Mock<ILogDataItemBroker> logDataItemBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly LogDataItemService logDataItemService;

    public LogDataItemServiceTests()
    {
        logDataItemBrokerMock = new Mock<ILogDataItemBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        logDataItemService = new LogDataItemService(
            logDataItemBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static LogDataItem CreateRandomLogDataItem(int id = 42, int logEntryId = 7)
    {
        LogDataItem logDataItem = Builder<LogDataItem>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.LogEntryId = logEntryId)
            .With(x => x.Name = $"Name-{Guid.NewGuid():N}")
            .With(x => x.Value = $"Value-{Guid.NewGuid():N}")
            .Build();

        return logDataItem;
    }

    private static DataLogDataItem CreateRandomDataLogDataItem(int id = 42, int logEntryId = 7)
    {
        DataLogDataItem logDataItem = Builder<DataLogDataItem>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.LogEntryId = logEntryId)
            .With(x => x.Name = $"Name-{Guid.NewGuid():N}")
            .With(x => x.Value = $"Value-{Guid.NewGuid():N}")
            .Build();

        return logDataItem;
    }
}












