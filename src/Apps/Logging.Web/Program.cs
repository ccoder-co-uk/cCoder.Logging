// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Logging;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Eventing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;


namespace Logging.Web;

public class Program
{
    private static ILogger log = null!;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        string ssoConnection = builder.Configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException("ConnectionStrings:SSO is required.");

        Config config = new();
        builder.Configuration.Bind(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddEventing();

        builder.Services.AddSecurityApi((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                builder.Configuration.GetSection("Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            builder.Services,
            coreConnection);

        builder.Services.AddLoggingWeb(loggingConfiguration =>
        {
            builder.Configuration.GetSection("LoggingConfiguration").Bind(loggingConfiguration);
            builder.Configuration.GetSection("ConnectionStrings").Bind(loggingConfiguration.ConnectionStrings);
            builder.Configuration.GetSection("Settings").Bind(loggingConfiguration.Settings);
            builder.Configuration.GetSection("Services").Bind(loggingConfiguration.Services);
            loggingConfiguration.DefaultAppId ??= GetConfiguredAppId(builder.Configuration);
            loggingConfiguration.DefaultAppDomain ??= "localhost";
        });

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Logging/swagger.json", "Logging API");
                options.SwaggerEndpoint("/swagger/Core/swagger.json", "Core API");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet("/Health", () => Results.Ok("OK"));
        app.StartLoggingWeb(log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(HandleUnhandledException);
        app.Run();
    }

    private static int? GetConfiguredAppId(IConfiguration configuration)
    {
        string configuredAppId = configuration.GetSection("Settings")["CacheSourceAppId"];
        return int.TryParse(configuredAppId, out int appId) ? appId : null;
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;
        context.Response.ContentType = "application/json";

        if (exception is null)
            return;

        log.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
        await context.Response.WriteAsync(
            "{ \"error\": \"" + exception.Message.Replace("\"", "\'") + "\" }");
    }
}