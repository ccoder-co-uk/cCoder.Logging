// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Logging.HostedServices.Hosting;

namespace Logging.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddLoggingHostedServicesApplication(configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseLoggingHostedServicesApplication();
        app.Run();
    }
}