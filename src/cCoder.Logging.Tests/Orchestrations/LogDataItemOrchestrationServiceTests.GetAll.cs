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
        logDataItemProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<LogDataItem> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        logDataItemProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}