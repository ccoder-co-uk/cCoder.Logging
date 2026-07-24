// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Logging.Web.Hosting;

namespace Logging.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddLoggingWebApplication(configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseLoggingWebApplication();
        app.Run();
    }
}