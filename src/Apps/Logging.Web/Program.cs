// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Builder;

namespace Logging.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddWeb(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseLoggingApplication()
            .Run();
    }
}