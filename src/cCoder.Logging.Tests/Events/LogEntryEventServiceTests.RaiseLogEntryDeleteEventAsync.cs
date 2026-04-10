using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Events;

public partial class LogEntryEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseLogEntryDeleteEventAsync()
    {
        // Given
        LogEntry entity = new();
        EventMessage<LogEntry> actualMessage = null;

        logEntryEventBrokerMock
            .Setup(x => x.RaiseLogEntryDeleteEventAsync(It.IsAny<EventMessage<LogEntry>>()))
            .Callback<EventMessage<LogEntry>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLogEntryDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeSameAs(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        logEntryEventBrokerMock.Verify(
            x => x.RaiseLogEntryDeleteEventAsync(It.IsAny<EventMessage<LogEntry>>()),
            Times.Once
        );
        logEntryEventBrokerMock.VerifyNoOtherCalls();
    }

}






