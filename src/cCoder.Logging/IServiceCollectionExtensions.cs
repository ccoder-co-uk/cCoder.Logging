using cCoder.Data.Models.Logging;
using cCoder.Logging.Api.OData;
using cCoder.Logging.Brokers;
using cCoder.Logging.Services;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Foundations.Events;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using EventLibrary;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Logging.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Logging;

public static class IServiceCollectionExtensions
{
    public static void AddLogging(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
    }

    public static void AddLoggingApi(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddLogging();
        services.AddApi("Logging", ConfigureLoggingApiModel, builder);
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<LogDataItem>();
        services.AddEventingForType<LogEntry>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemEventBroker, LogDataItemEventBroker>();
        services.AddTransient<ILogEntryEventBroker, LogEntryEventBroker>();
        services.AddTransient<ILogDataItemBroker, LogDataItemBroker>();
        services.AddTransient<ILogEntryBroker, LogEntryBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<ILoggingMetadataTypeService, LoggingMetadataTypeService>();
        services.AddTransient<ILogDataItemService, LogDataItemService>();
        services.AddTransient<ILogEntryService, LogEntryService>();
        services.AddTransient<ILogDataItemEventService, LogDataItemEventService>();
        services.AddTransient<ILogEntryEventService, LogEntryEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemOrchestrationService, LogDataItemOrchestrationService>();
        services.AddTransient<ILogEntryOrchestrationService, LogEntryOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemEventProcessingService, LogDataItemEventProcessingService>();
        services.AddTransient<ILogDataItemProcessingService, LogDataItemProcessingService>();
        services.AddTransient<ILogEntryEventProcessingService, LogEntryEventProcessingService>();
        services.AddTransient<ILogEntryProcessingService, LogEntryProcessingService>();
    }

    private static void ConfigureLoggingApiModel(ODataConventionModelBuilder builder) =>
        new LoggingModelBuilder(builder).Configure();

    private static void AddApi(
        this IServiceCollection services,
        string routePrefix,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, routePrefix, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();

        services.AddControllers().AddOData(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(1000)
                .AddRouteComponents($"Api/{routePrefix}", routeModel, batchHandler);

            if (builder is null)
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string routePrefix,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, routePrefix);
            options.DocInclusionPredicate(
                (documentName, apiDescription) =>
                    ShouldIncludeInDocument(documentName, apiDescription.RelativePath, routePrefix));

            if (useFullSchemaIds)
                options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string routePrefix)
    {
        options.SwaggerDoc(routePrefix, new OpenApiInfo
        {
            Title = $"{routePrefix} API definition",
            Version = routePrefix,
        });
        options.SwaggerDoc("Core", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "Core",
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "v1",
        });
    }

    private static bool ShouldIncludeInDocument(
        string documentName,
        string relativePath,
        string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(documentName, "v1", StringComparison.OrdinalIgnoreCase))
            documentName = "Core";

        string path = NormalizePath(relativePath);

        return string.Equals(documentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? MatchesContextRoute(path, "Core")
            : MatchesContextRoute(path, routePrefix);
    }

    private static bool MatchesContextRoute(string path, string context)
    {
        string prefix = $"/Api/{context}";
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            typeof(HttpContext),
            ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());
        services.AddScoped(typeof(HttpRequest), ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60);
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}






