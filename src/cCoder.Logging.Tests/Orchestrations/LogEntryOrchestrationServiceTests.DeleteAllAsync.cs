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
        logEntryProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        logEntryProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}