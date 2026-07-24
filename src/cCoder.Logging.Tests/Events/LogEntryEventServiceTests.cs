// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Logging.Brokers;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogEntryEventServiceTests
{
    private readonly Mock<ILogEntryEventBroker> logEntryEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Logging.Services.Foundations.Events.LogEntryEventService service;
    private const string CurrentUserId = "test-user";

    public LogEntryEventServiceTests()
    {
        logEntryEventBrokerMock = new Mock<ILogEntryEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Logging.Services.Foundations.Events.LogEntryEventService(
            logEntryEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}