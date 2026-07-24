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
        logDataItemProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        LogDataItem result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        logDataItemProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        logDataItemProcessingServiceMock.VerifyNoOtherCalls();
        logDataItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}