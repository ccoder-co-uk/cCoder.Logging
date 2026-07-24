// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogEntryEventServiceTests
{
    private readonly Mock<ILogEntryEventBroker> logEntryEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.Logging.Services.Foundations.Events.LogEntryEventService service;
    private const string CurrentUserId = "test-user";

    public LogEntryEventServiceTests()
    {
        logEntryEventBrokerMock = new Mock<ILogEntryEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);

        authInfoBrokerMock
            .Setup(expression: broker => broker.SelectCurrentSsoUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Logging.Services.Foundations.Events.LogEntryEventService(
            logEntryEventBroker: logEntryEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);
    }
}