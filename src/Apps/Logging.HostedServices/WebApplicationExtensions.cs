// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging;
using cCoder.Logging.Models;

namespace Logging.HostedServices;

public static class WebApplicationExtensions
{
    public static WebApplication UseLoggingHostedServicesApplication(
        this WebApplication app)
    {
        LoggingConfiguration configuration =
            app.Services.GetRequiredService<LoggingConfiguration>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapGet(
            pattern: "/",
            handler: () => Results.Text(
                content: GetHostedServicesReport(
                    configuration: configuration),
                contentType: "text/plain"));
        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "OK"));
        app.StartLoggingHostedServices();

        return app;
    }

    private static string GetHostedServicesReport(
        LoggingConfiguration configuration) =>
        "Logging Hosted Services\r\n"
        + "\r\n"
        + "Hosted services:\r\n"
        + "- LogRetentionCleaner: removes log entries older than "
        + $"{GetRetentionDays(configuration: configuration)} days every "
        + $"{GetRetentionIntervalMinutes(configuration: configuration)} minutes.\r\n"
        + "\r\n"
        + $"DB storage enabled: {configuration.StoreLogEntries}\r\n"
        + $"SignalR streaming enabled: {configuration.StreamLogEntries}\r\n";

    private static int GetRetentionDays(
        LoggingConfiguration configuration) =>
        configuration.RetentionDays <= 0
            ? 30
            : configuration.RetentionDays;

    private static int GetRetentionIntervalMinutes(
        LoggingConfiguration configuration) =>
        configuration.RetentionIntervalMinutes <= 0
            ? 60
            : configuration.RetentionIntervalMinutes;
}