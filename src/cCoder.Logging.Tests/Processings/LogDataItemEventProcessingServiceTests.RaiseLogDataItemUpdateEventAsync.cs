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
    public async Task ShouldPassThroughCallWhenRaiseLogDataItemUpdateEventAsync()
    {
        // Given
        LogDataItem entity = CreateRandomLogDataItem();

        logDataItemEventServiceMock
            .Setup(expression: x => x.RaiseLogDataItemUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLogDataItemUpdateEventAsync(entity: entity);

        // Then
        logDataItemEventServiceMock.Verify(expression: x => x.RaiseLogDataItemUpdateEventAsync(entity: entity), times: Times.Once);
        logDataItemEventServiceMock.VerifyNoOtherCalls();
    }

}