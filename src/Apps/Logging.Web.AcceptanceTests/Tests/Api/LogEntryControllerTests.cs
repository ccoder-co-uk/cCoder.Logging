// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Json;
using cCoder.Data;
using cCoder.Data.Models.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Logging.Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class LogEntryControllerTests(WebAcceptanceFixture fixture)
{
    private const string LogEntryRoute = "/Api/Logging/LogEntry";
    private HttpClient Client { get; } = fixture.Client;

    private async Task<LogEntry> FindLogEntryAsync(string message)
    {
        // Given
        IServiceScope scope = fixture.Factory.Services.CreateScope();

        CoreDataContext context = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        // When
        LogEntry logEntry = await context.Logs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: logEntry => logEntry.Message == message);

        // Then
        await context.DisposeAsync();
        scope.Dispose();

        return logEntry;
    }

    private static LogEntry CreateLogEntry() =>
        new()
        {
            AppId = 1,
            AppName = "localhost",
            TypeName = "Acceptance",
            Message = $"acceptance-log-{Guid.NewGuid():N}",
            Level = (int)cCoder.Logging.Models.LoggingLevel.Info,
            Date = DateTime.UtcNow,
            Data = []
        };
}