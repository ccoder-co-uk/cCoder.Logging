// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        logger.LogInformation(message: "{Message}", args: expectedMessage);
        LogEntry storedLogEntry = await WaitForLogEntryAsync(message: expectedMessage);

        // Then

        storedLogEntry.Should()
            .NotBeNull();

        storedLogEntry.AppId.Should()
            .Be(expected: 1);

        storedLogEntry.AppName.Should()
            .Be(expected: "localhost");
    }

    private async Task<LogEntry> WaitForLogEntryAsync(string message)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            LogEntry logEntry = await FindLogEntryAsync(message: message);

            if (logEntry is not null)
            {
                return logEntry;
            }

            await Task.Delay(millisecondsDelay: 100);
        }

        return null;
    }
}