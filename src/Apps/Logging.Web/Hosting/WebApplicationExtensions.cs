// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Eventing;
using cCoder.Logging;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;

namespace Logging.Web.Hosting;

internal static class WebApplicationExtensions
{
    private static ILogger log = null!;

    internal static IServiceCollection AddLoggingWebApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string coreConnection = configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        string ssoConnection = configuration.GetConnectionString(name: "SSO")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:SSO is required.");

        Config config = new();
        configuration.Bind(instance: config);
        services.AddSingleton(implementationInstance: config);
        services.AddEventing();

        services.AddSecurityApi(configAction: (securityServices, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(
                services: securityServices,
                connectionString: ssoConnection);

            securityConfig.UseAESHMMACPasswordEncryption(
                services: securityServices,
                decryptionKey: configuration.GetSection(key: "Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            services: services,
            connectionString: coreConnection);

        services.AddLoggingWeb(configure: loggingConfiguration =>
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

    internal static WebApplication UseLoggingWebApplication(
        this WebApplication app)
    {
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(url: "/swagger/Logging/swagger.json", name: "Logging API");
                options.SwaggerEndpoint(url: "/swagger/Core/swagger.json", name: "Core API");
                options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/Health", handler: () => Results.Ok(value: "OK"));
        app.StartLoggingWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);

        return app;
    }

    private static int? GetConfiguredAppId(IConfiguration configuration)
    {
        string configuredAppId = configuration.GetSection(key: "Settings")["CacheSourceAppId"];

        return int.TryParse(s: configuredAppId, result: out int appId)
            ? appId
            : null;
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException)
                ? 401
                : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError(
            message: "{Message}\n{StackTrace}",
            exception.Message,
            exception.StackTrace);

        await context.Response.WriteAsync(
            text: "{ \"error\": \"" + exception.Message.Replace(
                oldValue: "\"",
                newValue: "\'") + "\" }");
    }
}