using cCoder.Data;
using cCoder.Logging.Brokers;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogDataItemEventServiceTests
{
    private readonly Mock<ILogDataItemEventBroker> logDataItemEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Logging.Services.Foundations.Events.LogDataItemEventService service;
    private const string CurrentUserId = "test-user";

    public LogDataItemEventServiceTests()
    {
        logDataItemEventBrokerMock = new Mock<ILogDataItemEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.Logging.Services.Foundations.Events.LogDataItemEventService(
            logDataItemEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}








