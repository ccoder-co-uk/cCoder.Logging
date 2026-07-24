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
    public async Task ShouldPassThroughCallWhenRaiseLogEntryUpdateEventAsync()
    {
        // Given
        LogEntry entity = CreateRandomLogEntry();

        logEntryEventServiceMock
            .Setup(expression: x => x.RaiseLogEntryUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLogEntryUpdateEventAsync(entity: entity);

        // Then
        logEntryEventServiceMock.Verify(expression: x => x.RaiseLogEntryUpdateEventAsync(entity: entity), times: Times.Once);
        logEntryEventServiceMock.VerifyNoOtherCalls();
    }

}