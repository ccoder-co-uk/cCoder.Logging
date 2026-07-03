using cCoder.Data.Models.Logging;
using cCoder.Logging.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging.Orchestrations;

public partial class LogEntryCaptureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldStreamPersistAndRaiseEventWhenCaptureAsync()
    {
        // Given
        LogEntryCaptureRequest request = CreateRequest();
        LogEntry storedLogEntry = null;

        logEntryStreamBrokerMock
            .Setup(broker => broker.StreamAsync("localhost", "information", request.Message))
            .Returns(ValueTask.CompletedTask);
        logEntryProcessingServiceMock
            .Setup(service => service.ResolveAppId("localhost"))
            .Returns((int?)7);
        logEntryProcessingServiceMock
            .Setup(service => service.AddSystemAsync(It.IsAny<LogEntry>()))
            .Callback<LogEntry>(logEntry => storedLogEntry = logEntry)
            .ReturnsAsync((LogEntry logEntry) => logEntry);
        logEntryEventProcessingServiceMock
            .Setup(service => service.RaiseLogEntryAddEventAsync(It.IsAny<LogEntry>()))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.CaptureAsync(request);

        // Then
        storedLogEntry.Should().NotBeNull();
        storedLogEntry.AppId.Should().Be(7);
        storedLogEntry.AppName.Should().Be("localhost");
        storedLogEntry.Message.Should().Be(request.Message);
        logEntryStreamBrokerMock.VerifyAll();
        logEntryProcessingServiceMock.Verify(service => service.ResolveAppId("localhost"), Times.Once);
        logEntryProcessingServiceMock.Verify(service => service.AddSystemAsync(It.IsAny<LogEntry>()), Times.Once);
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.Verify(service => service.RaiseLogEntryAddEventAsync(It.IsAny<LogEntry>()), Times.Once);
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldOnlyStreamWhenDatabaseStorageDisabledForCaptureAsync()
    {
        // Given
        LogEntryCaptureRequest request = CreateRequest();
        configuration.StoreLogEntries = false;
        logEntryStreamBrokerMock
            .Setup(broker => broker.StreamAsync("localhost", "information", request.Message))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.CaptureAsync(request);

        // Then
        logEntryStreamBrokerMock.VerifyAll();
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSkipIgnoredCategoriesWhenCaptureAsync()
    {
        // Given
        LogEntryCaptureRequest request = CreateRequest();
        request.CategoryName = "Microsoft.EntityFrameworkCore.Database.Command";

        // When
        await orchestrationService.CaptureAsync(request);

        // Then
        logEntryStreamBrokerMock.VerifyNoOtherCalls();
        logEntryProcessingServiceMock.VerifyNoOtherCalls();
        logEntryEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}
