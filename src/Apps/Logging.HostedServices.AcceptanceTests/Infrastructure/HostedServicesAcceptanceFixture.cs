// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Logging.HostedServices.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    internal HostedServicesAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Factory = new HostedServicesAcceptanceFactory();

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }
}