using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Services.Foundations;


namespace cCoder.Logging;

public static class WebApplicationExtensions
{
    private const string MetadataScope = "Logging";

    public static WebApplication UseLoggingExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation("Initialising Logging");
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




