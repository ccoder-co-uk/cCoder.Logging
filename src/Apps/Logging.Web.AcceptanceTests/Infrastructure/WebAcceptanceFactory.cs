// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Logging.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Logging.Web.AcceptanceTests.Models;


namespace Logging.Web.AcceptanceTests.Infrastructure;

internal sealed class WebAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(key: "Logging:ConnectionString", value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(key: "Data:ConnectionString", value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(key: "Security:ConnectionString", value: settings.SsoConnectionString),
                new KeyValuePair<string, string>(key: "Security:DecryptionKey", value: settings.DecryptionKey),
                new KeyValuePair<string, string>(key: "Eventing:ProviderType", value: string.Empty),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<CoreDataContext>();
            services.RemoveAll<IDbContextFactory<CoreDataContext>>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<DataConfiguration>();

            services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: settings.SsoConnectionString)
            );

            services.AddData(
                configuration: new DataConfiguration
                {
                    ConnectionString = settings.CoreConnectionString
                });
        });
    }
}