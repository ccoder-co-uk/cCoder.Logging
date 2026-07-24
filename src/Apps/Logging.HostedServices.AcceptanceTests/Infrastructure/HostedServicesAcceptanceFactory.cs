// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Logging.HostedServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Logging.HostedServices.AcceptanceTests.Infrastructure;

internal sealed class HostedServicesAcceptanceFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Acceptance");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string>(
                    "ConnectionStrings:Core",
                    configuration["CCODER_ACCEPTANCE_CORE_CONNECTION_STRING"] ?? string.Empty),
                new KeyValuePair<string, string>("LoggingConfiguration:StoreLogEntries", "false"),
                new KeyValuePair<string, string>("LoggingConfiguration:StreamLogEntries", "false"),
            ]);
        });
    }
}