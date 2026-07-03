using cCoder.Data;
using cCoder.Data.Models.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Logging.Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class LogEntryCaptureTests(WebAcceptanceFixture fixture)
{
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

    private ILogger<LogEntryCaptureTests> CreateLogger() =>
        fixture.Factory.Services.GetRequiredService<ILogger<LogEntryCaptureTests>>();
}
