// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogEntryEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseLogEntryDeleteEventAsync()
    {
        // Given
        LogEntry entity = CreateRandomLogEntry();
        logEntryEventServiceMock
            .Setup(x => x.RaiseLogEntryDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLogEntryDeleteEventAsync(entity);

        // Then
        logEntryEventServiceMock.Verify(x => x.RaiseLogEntryDeleteEventAsync(entity), Times.Once);
        logEntryEventServiceMock.VerifyNoOtherCalls();
    }

}