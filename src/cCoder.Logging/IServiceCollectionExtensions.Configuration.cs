// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.Logging;

public static partial class IServiceCollectionExtensions
{
    internal static void RegisterLoggingConfiguration(
        this IServiceCollection services,
        LoggingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.AddSingleton(implementationInstance: configuration);

        services.AddData(
            configuration: new cCoder.Data.Models.DataConfiguration
            {
                ConnectionString = configuration.ConnectionString,
                DebugInfo = configuration.DebugInfo,
                LogSQL = configuration.LogSQL,
            });

        services.AddEventProviders(eventProviders: configuration.EventProviders ?? []);
    }

    internal static void AddLoggingApi(
        this IServiceCollection services,
        LoggingConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        const string documentName = "Logging";

        Action<ODataConventionModelBuilder> configureModel =
            static modelBuilder =>
                modelBuilder.ConfigureLoggingApiModel();

        services.AddSingleton<Action<ODataConventionModelBuilder>>(implementationInstance: configureModel);

        if (builder is not null)
        {
            configureModel(obj: builder);
        }

        services.AddAspNet();

        if (builder is null)
        {
            services.AddApiDocumentation(
                documentName: documentName,
                configuration: configuration);
        }

        IEdmModel routeModel =
            services.BuildRouteModel(
                configureModel: configureModel);
        DefaultODataBatchHandler batchHandler = new();

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        IMvcBuilder mvcBuilder = services.AddControllers();

        mvcBuilder.AddOData(setupAction: options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;

            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(maxTopValue: 1000)
                .AddRouteComponents(routePrefix: rootPath, model: routeModel, batchHandler: batchHandler);
        });
    }

    private static void AddApiDocumentation(
        this IServiceCollection services,
        string documentName,
        LoggingConfiguration configuration) =>
        services.AddSwaggerGen(setupAction: options =>
        {
            options.ResolveConflictingActions(resolver: apiDescriptions => apiDescriptions.First());
            services.AddSwaggerDocument(
                options: options,
                documentName: documentName);

            options.DocInclusionPredicate(
                predicate: (swaggerDocumentName, apiDescription) =>
                    services.ShouldIncludeInDocument(
                        swaggerDocumentName: swaggerDocumentName,
                        relativePath: apiDescription.RelativePath,
                        documentName: documentName,
                        configuration: configuration));

            options.AddSecurityDefinition(name: "bearer", securityScheme: new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });

    private static void AddSwaggerDocument(
        this IServiceCollection services,
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string documentName) =>
        options.SwaggerDoc(name: documentName, info: new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });

    private static bool ShouldIncludeInDocument(
        this IServiceCollection services,
        string swaggerDocumentName,
        string relativePath,
        string documentName,
        LoggingConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(value: relativePath))
        {
            return false;
        }

        string path =
            services.NormalizePath(
                relativePath: relativePath);

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(
                a: swaggerDocumentName,
                b: documentName,
                comparisonType: StringComparison.OrdinalIgnoreCase)
            && services.MatchesContextRoute(
                path: path,
                rootPath: rootPath);
    }

    private static bool MatchesContextRoute(
        this IServiceCollection services,
        string path,
        string rootPath)
    {
        string normalizedPath =
            services.NormalizePath(
                relativePath: rootPath);

        return path.Equals(value: normalizedPath, comparisonType: StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(value: $"{normalizedPath}/", comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(
        this IServiceCollection services,
        string relativePath) =>
        relativePath.StartsWith(value: '/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(
        this IServiceCollection services,
        Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(
        this IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddScoped(
            serviceType: typeof(HttpContext),
            implementationFactory: ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());

        services.AddScoped(serviceType: typeof(HttpRequest), implementationFactory: ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();

        services.AddHsts(configureOptions: options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(minutes: 60);
        });

        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
        services.AddRazorPages();

        services.Configure<KestrelServerOptions>(configureOptions: options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}