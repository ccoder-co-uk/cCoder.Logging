// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Logging.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddLoggingWeb(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseLoggingApplication()
            .Run();
    }
}