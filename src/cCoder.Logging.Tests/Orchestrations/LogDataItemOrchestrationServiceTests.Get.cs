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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        LogDataItem entity = CreateRandomLogDataItem();

        logDataItemProcessingServiceMock.Setup(expression: x => x.GetLogDataItem(logDataItemId: id))
            .Returns(value: entity);

        // When
        LogDataItem result = orchestrationService.GetLogDataItem(logDataItemId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        logDataItemProcessingServiceMock.Verify(expression: x => x.GetLogDataItem(logDataItemId: id), times: Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}