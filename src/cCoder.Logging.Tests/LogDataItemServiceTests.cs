// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations;
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
        logDataItemBrokerMock = new Mock<ILogDataItemBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        logDataItemService = new LogDataItemService(
            logDataItemBroker: logDataItemBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
    }

    private static LogDataItem CreateRandomLogDataItem(int id = 42, int logEntryId = 7)
    {
        LogDataItem logDataItem = Builder<LogDataItem>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.LogEntryId = logEntryId)
            .With(func: x => x.Name = $"Name-{Guid.NewGuid():N}")
            .With(func: x => x.Value = $"Value-{Guid.NewGuid():N}")
            .Build();

        return logDataItem;
    }

    private static DataLogDataItem CreateRandomDataLogDataItem(int id = 42, int logEntryId = 7)
    {
        DataLogDataItem logDataItem = Builder<DataLogDataItem>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.LogEntryId = logEntryId)
            .With(func: x => x.Name = $"Name-{Guid.NewGuid():N}")
            .With(func: x => x.Value = $"Value-{Guid.NewGuid():N}")
            .Build();

        return logDataItem;
    }
}