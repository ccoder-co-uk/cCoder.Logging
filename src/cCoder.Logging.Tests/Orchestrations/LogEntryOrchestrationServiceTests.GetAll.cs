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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<LogEntry> entities = new[] { CreateRandomLogEntry() }.AsQueryable();

        logEntryProcessingServiceMock.Setup(expression: x => x.GetAllLogEntries(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<LogEntry> result = orchestrationService.GetAllLogEntries(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        logEntryProcessingServiceMock.Verify(expression: x => x.GetAllLogEntries(ignoreFilters: true), times: Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}