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
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(
key: "ConnectionStrings:Core",
value: configuration["CCODER_ACCEPTANCE_CORE_CONNECTION_STRING"] ?? string.Empty),
                new KeyValuePair<string, string>(key: "LoggingConfiguration:StoreLogEntries", value: "false"),
                new KeyValuePair<string, string>(key: "LoggingConfiguration:StreamLogEntries", value: "false"),
            ]);
        });
    }
}