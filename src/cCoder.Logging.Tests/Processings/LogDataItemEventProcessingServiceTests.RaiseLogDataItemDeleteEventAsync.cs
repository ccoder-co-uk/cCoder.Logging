// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogDataItemEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseLogDataItemDeleteEventAsync()
    {
        // Given
        LogDataItem entity = CreateRandomLogDataItem();
        logDataItemEventServiceMock
            .Setup(x => x.RaiseLogDataItemDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLogDataItemDeleteEventAsync(entity);

        // Then
        logDataItemEventServiceMock.Verify(x => x.RaiseLogDataItemDeleteEventAsync(entity), Times.Once);
        logDataItemEventServiceMock.VerifyNoOtherCalls();
    }

}