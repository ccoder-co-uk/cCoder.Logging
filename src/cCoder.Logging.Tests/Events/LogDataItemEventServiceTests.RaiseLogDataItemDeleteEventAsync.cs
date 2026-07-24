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

public partial class LogDataItemEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseLogDataItemDeleteEventAsync()
    {
        // Given
        LogDataItem entity = new();
        EventMessage<LogDataItem> actualMessage = null;

        logDataItemEventBrokerMock
            .Setup(expression: x => x.RaiseLogDataItemDeleteEventAsync(message: It.IsAny<EventMessage<LogDataItem>>()))
            .Callback<EventMessage<LogDataItem>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLogDataItemDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeSameAs(expected: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        logDataItemEventBrokerMock.Verify(
expression: x => x.RaiseLogDataItemDeleteEventAsync(message: It.IsAny<EventMessage<LogDataItem>>()),
times: Times.Once
        );

        logDataItemEventBrokerMock.VerifyNoOtherCalls();
    }

}