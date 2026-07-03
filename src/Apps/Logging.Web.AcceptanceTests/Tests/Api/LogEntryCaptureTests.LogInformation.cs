using cCoder.Data.Models.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogEntryCaptureTests
{
    [Fact]
    public async Task ShouldPersistLogEntryWhenLogInformation()
    {
        // Given
        string expectedMessage = $"captured-log-{Guid.NewGuid():N}";
        ILogger<LogEntryCaptureTests> logger = CreateLogger();

        // When
        logger.LogInformation("{Message}", expectedMessage);
        LogEntry storedLogEntry = await WaitForLogEntryAsync(expectedMessage);

        // Then
        storedLogEntry.Should().NotBeNull();
        storedLogEntry.AppId.Should().Be(1);
        storedLogEntry.AppName.Should().Be("localhost");
    }

    private async Task<LogEntry> WaitForLogEntryAsync(string message)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            LogEntry logEntry = await FindLogEntryAsync(message);

            if (logEntry is not null)
                return logEntry;

            await Task.Delay(100);
        }

        return null;
    }
}
