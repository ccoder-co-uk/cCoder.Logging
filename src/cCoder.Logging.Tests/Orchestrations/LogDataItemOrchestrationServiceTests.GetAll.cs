// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogDataItemOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<LogDataItem> entities = new[] { CreateRandomLogDataItem() }.AsQueryable();

        logDataItemProcessingServiceMock.Setup(expression: x => x.GetAllLogDataItems(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<LogDataItem> result = orchestrationService.GetAllLogDataItems(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        logDataItemProcessingServiceMock.Verify(expression: x => x.GetAllLogDataItems(ignoreFilters: true), times: Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}