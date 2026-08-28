// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Apps.Shared.Testing;
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
            AcceptanceTestConfiguration configuration =
                AcceptanceTestConfiguration.Load();

            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(
key: "CoreData:ConnectionString",
value: configuration.CoreConnectionString),
                new KeyValuePair<string, string>(
                    key: "Logging:StoreLogEntries",
                    value: "false"),
                new KeyValuePair<string, string>(
                    key: "Logging:StreamLogEntries",
                    value: "false"),
            ]);
        });
    }
}