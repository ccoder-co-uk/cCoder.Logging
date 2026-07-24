// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogDataItemEventServiceTests
{
    private readonly Mock<ILogDataItemEventBroker> logDataItemEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.Logging.Services.Foundations.Events.LogDataItemEventService service;
    private const string CurrentUserId = "test-user";

    public LogDataItemEventServiceTests()
    {
        logDataItemEventBrokerMock = new Mock<ILogDataItemEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);

        authInfoBrokerMock
            .Setup(expression: broker => broker.SelectCurrentSsoUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Logging.Services.Foundations.Events.LogDataItemEventService(
            logDataItemEventBroker: logDataItemEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);
    }
}