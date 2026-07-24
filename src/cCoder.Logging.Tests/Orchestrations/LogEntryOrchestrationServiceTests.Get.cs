// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        LogEntry entity = CreateRandomLogEntry();

        logEntryProcessingServiceMock.Setup(expression: x => x.GetLogEntry(logEntryId: id))
            .Returns(value: entity);

        // When
        LogEntry result = orchestrationService.GetLogEntry(logEntryId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        logEntryProcessingServiceMock.Verify(expression: x => x.GetLogEntry(logEntryId: id), times: Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}