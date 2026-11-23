// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Eventing;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Models.OData;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Exposures;


namespace cCoder.Logging;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "Logging";
    private const string LoggingExposureKey = "cCoder.Logging.ExposureRegistered";
    private const string LogStreamReceiverKey = "cCoder.Logging.StreamReceiverRegistered";

    public static WebApplication StartLoggingWeb(this WebApplication app, ILogger log = null) =>
        app.UseLoggingExposure(log);

    public static WebApplication StartLoggingHostedServices(
        this WebApplication app)
    {
        app.UseLoggingExposure();

        if (!((IApplicationBuilder)app).Properties.TryAdd(key: LogStreamReceiverKey, value: true))
        {
            return app;
        }

        using IServiceScope scope = app.Services.CreateScope();

        scope.ServiceProvider.GetService<IEventHub>()?
            .ListenToEvent<LogStreamMessage[], ILogEntryStreamService>(
                name: LogStreamMessage.EventName,
                handler: static async (service, entries) =>
                {
                    if (service.ShouldStreamLogEntries())
                    {
                        foreach (LogStreamMessage entry in entries)
                        {
                            await service.StreamLogEntryAsync(
                                thread: entry.Domain,
                                level: entry.Level,
                                message: entry.Message);
                        }
                    }
                });

        return app;
    }

    private static WebApplication UseLoggingExposure(this WebApplication app, ILogger log = null)
    {
        if (!((IApplicationBuilder)app).Properties.TryAdd(key: LoggingExposureKey, value: true))
        {
            return app;
        }

        log?.LogInformation("Initialising Logging");
        app.UseMiddleware<RequestLoggingMiddleware>();
        PopulateMetadataTypeCache(app);
        app.MapHub<LogHub>("/Api/Hubs/Logs");
        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(MetadataScope))
        {
            metadataTypeCache.Set(
                MetadataScope,
                app.Services
                    .GetRequiredService<ILoggingMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}