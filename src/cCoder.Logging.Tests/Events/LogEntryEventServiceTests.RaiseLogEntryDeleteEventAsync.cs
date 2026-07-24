// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Eventing.Models;
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
            .Setup(expression: x => x.RaiseLogEntryDeleteEventAsync(message: It.IsAny<EventMessage<LogEntry>>()))
            .Callback<EventMessage<LogEntry>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLogEntryDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeSameAs(expected: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        logEntryEventBrokerMock.Verify(
expression: x => x.RaiseLogEntryDeleteEventAsync(message: It.IsAny<EventMessage<LogEntry>>()),
times: Times.Once
        );

        logEntryEventBrokerMock.VerifyNoOtherCalls();
    }

}