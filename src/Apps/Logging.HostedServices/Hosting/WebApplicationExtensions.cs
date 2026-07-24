// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Logging;
using cCoder.Logging.Models;

namespace Logging.HostedServices.Hosting;

internal static class WebApplicationExtensions
{
    internal static IServiceCollection AddLoggingHostedServicesApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string coreConnection = configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        services.AddEventing();
        services.AddCoreData(connectionString: coreConnection);

        services.AddLoggingHostedServices(configure: loggingConfiguration =>
        {
            configuration.GetSection(key: "LoggingConfiguration")
                .Bind(instance: loggingConfiguration);

            configuration.GetSection(key: "ConnectionStrings")
                .Bind(instance: loggingConfiguration.ConnectionStrings);

            configuration.GetSection(key: "Settings")
                .Bind(instance: loggingConfiguration.Settings);

            configuration.GetSection(key: "Services")
                .Bind(instance: loggingConfiguration.Services);

            loggingConfiguration.DefaultAppId ??= GetConfiguredAppId(
                configuration: configuration);

            loggingConfiguration.DefaultAppDomain ??= "localhost";
        });

        return services;
    }

    internal static WebApplication UseLoggingHostedServicesApplication(
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
                content: GetHostedServicesReport(configuration: configuration),
                contentType: "text/plain"));

        app.MapGet(pattern: "/Health", handler: () => Results.Ok(value: "OK"));
        app.StartLoggingHostedServices();

        return app;
    }

    private static int? GetConfiguredAppId(IConfiguration configuration)
    {
        string configuredAppId = configuration.GetSection(key: "Settings")["CacheSourceAppId"];

        return int.TryParse(s: configuredAppId, result: out int appId)
            ? appId
            : null;
    }

    private static string GetHostedServicesReport(LoggingConfiguration configuration) =>
        "Logging Hosted Services\r\n" +
        "\r\n" +
        "Hosted services:\r\n" +
        $"- LogRetentionCleaner: removes log entries older than " +
        $"{GetRetentionDays(configuration: configuration)} days every " +
        $"{GetRetentionIntervalMinutes(configuration: configuration)} minutes.\r\n" +
        "\r\n" +
        $"DB storage enabled: {configuration.StoreLogEntries}\r\n" +
        $"SignalR streaming enabled: {configuration.StreamLogEntries}\r\n";

    private static int GetRetentionDays(LoggingConfiguration configuration) =>
        configuration.RetentionDays <= 0
            ? 30
            : configuration.RetentionDays;

    private static int GetRetentionIntervalMinutes(LoggingConfiguration configuration) =>
        configuration.RetentionIntervalMinutes <= 0
            ? 60
            : configuration.RetentionIntervalMinutes;
}