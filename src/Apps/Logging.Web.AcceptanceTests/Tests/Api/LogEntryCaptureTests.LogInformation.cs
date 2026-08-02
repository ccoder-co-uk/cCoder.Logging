// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogEntryCaptureTests
{
    [Fact]
    public async Task ShouldPersistExactlyOneLogEntryForCompletedHttpRequest()
    {
        // Given
        string requestPath = $"/request-log-{Guid.NewGuid():N}";

        // When
        using HttpResponseMessage response = await fixture.Client.GetAsync(
            requestUri: requestPath);

        int storedCount = await WaitForRequestLogCountAsync(
            requestPath: requestPath);

        // Then

        storedCount.Should()
            .Be(expected: 1);
    }

    [Fact]
    public async Task ShouldNotPersistLogEntryWhenLogInformation()
    {
        // Given
        string expectedMessage = $"captured-log-{Guid.NewGuid():N}";
        ILogger<LogEntryCaptureTests> logger = CreateLogger();

        // When
        logger.LogInformation(message: "{Message}", args: expectedMessage);
        await Task.Delay(millisecondsDelay: 250);
        LogEntry storedLogEntry = await FindLogEntryAsync(message: expectedMessage);

        // Then

        storedLogEntry.Should()
            .BeNull();
    }

    private async Task<int> WaitForRequestLogCountAsync(string requestPath)
    {
        for (int attempt = 0; attempt < 40; attempt++)
        {
            using IServiceScope scope = fixture.Factory.Services.CreateScope();

            using cCoder.Data.CoreDataContext context = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            int storedCount = await context.Logs
                .IgnoreQueryFilters()
                .CountAsync(predicate: entry =>
                    entry.Message.Contains(value: requestPath));

            if (storedCount > 0)
            {
                return storedCount;
            }

            await Task.Delay(millisecondsDelay: 250);
        }

        return 0;
    }
}