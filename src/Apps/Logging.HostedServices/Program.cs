// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Logging;
using cCoder.Logging.Models;

namespace Logging.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        builder.Services.AddEventing();
        builder.Services.AddCoreData(coreConnection);
        builder.Services.AddLoggingHostedServices(loggingConfiguration =>
        {
            builder.Configuration.GetSection("LoggingConfiguration").Bind(loggingConfiguration);
            builder.Configuration.GetSection("ConnectionStrings").Bind(loggingConfiguration.ConnectionStrings);
            builder.Configuration.GetSection("Settings").Bind(loggingConfiguration.Settings);
            builder.Configuration.GetSection("Services").Bind(loggingConfiguration.Services);
            loggingConfiguration.DefaultAppId ??= GetConfiguredAppId(builder.Configuration);
            loggingConfiguration.DefaultAppDomain ??= "localhost";
        });

        WebApplication app = builder.Build();
        LoggingConfiguration configuration = app.Services.GetRequiredService<LoggingConfiguration>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapGet("/", () => Results.Text(GetHostedServicesReport(configuration), "text/plain"));
        app.MapGet("/Health", () => Results.Ok("OK"));
        app.StartLoggingHostedServices();
        app.Run();
    }

    private static int? GetConfiguredAppId(IConfiguration configuration)
    {
        string configuredAppId = configuration.GetSection("Settings")["CacheSourceAppId"];
        return int.TryParse(configuredAppId, out int appId) ? appId : null;
    }

    private static string GetHostedServicesReport(LoggingConfiguration configuration) =>
        "Logging Hosted Services\r\n" +
        "\r\n" +
        "Hosted services:\r\n" +
        $"- LogRetentionCleaner: removes log entries older than {GetRetentionDays(configuration)} days every {GetRetentionIntervalMinutes(configuration)} minutes.\r\n" +
        "\r\n" +
        $"DB storage enabled: {configuration.StoreLogEntries}\r\n" +
        $"SignalR streaming enabled: {configuration.StreamLogEntries}\r\n";

    private static int GetRetentionDays(LoggingConfiguration configuration) =>
        configuration.RetentionDays <= 0 ? 30 : configuration.RetentionDays;

    private static int GetRetentionIntervalMinutes(LoggingConfiguration configuration) =>
        configuration.RetentionIntervalMinutes <= 0 ? 60 : configuration.RetentionIntervalMinutes;
}