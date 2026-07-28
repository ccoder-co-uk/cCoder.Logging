// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;

namespace Logging.Web;

public static class WebApplicationExtensions
{
    public static WebApplication UseLoggingApplication(
        this WebApplication app)
    {
        ILogger log =
            app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSession();
        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
                options.SwaggerEndpoint(
                    url: "/swagger/Logging/swagger.json",
                    name: "Logging API"))
            .UseODataBatching()
            .UseODataRouteDebug();
        app.UseRouting();
        app.MapControllers();
        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "OK"));
        app.StartLoggingWeb(log: log);
        app.UseCors(configurePolicy: policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });
        app.UseExceptionHandler(
            configure: errorApp =>
                errorApp.Run(handler: context =>
                    HandleUnhandledException(
                        context: context,
                        log: log)));

        return app;
    }

    private static async Task HandleUnhandledException(
        HttpContext context,
        ILogger log)
    {
        Exception exception =
            context.Features
                .Get<IExceptionHandlerPathFeature>()
                ?.Error;

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
            text: "{ \"error\": \""
                + exception.Message.Replace(
                    oldValue: "\"",
                    newValue: "\'")
                + "\" }");
    }
}