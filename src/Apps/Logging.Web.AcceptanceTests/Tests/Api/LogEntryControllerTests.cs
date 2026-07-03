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
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using CoreDataContext context = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await context.Logs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(logEntry => logEntry.Message == message);
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
