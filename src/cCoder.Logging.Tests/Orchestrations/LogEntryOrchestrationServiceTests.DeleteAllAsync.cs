// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        LogEntry[] entities = [CreateRandomLogEntry()];

        logEntryProcessingServiceMock.Setup(expression: x => x.DeleteAllLogEntryAsync(deletedLogEntries: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllLogEntryAsync(deletedLogEntries: entities);

        // Then
        logEntryProcessingServiceMock.Verify(expression: x => x.DeleteAllLogEntryAsync(deletedLogEntries: entities), times: Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}