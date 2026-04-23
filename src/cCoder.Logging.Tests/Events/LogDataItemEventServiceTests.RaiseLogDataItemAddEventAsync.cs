using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogDataItemEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseLogDataItemAddEventAsync()
    {
        // Given
        LogDataItem entity = new();
        EventMessage<LogDataItem> actualMessage = null;

        logDataItemEventBrokerMock
            .Setup(x => x.RaiseLogDataItemAddEventAsync(It.IsAny<EventMessage<LogDataItem>>()))
            .Callback<EventMessage<LogDataItem>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLogDataItemAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeSameAs(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        logDataItemEventBrokerMock.Verify(
            x => x.RaiseLogDataItemAddEventAsync(It.IsAny<EventMessage<LogDataItem>>()),
            Times.Once
        );
        logDataItemEventBrokerMock.VerifyNoOtherCalls();
    }

}






