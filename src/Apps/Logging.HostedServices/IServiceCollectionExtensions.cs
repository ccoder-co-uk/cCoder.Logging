// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using Logging.HostedServices.Models;

namespace Logging.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration hostedConfiguration = new();
        configuration.Bind(instance: hostedConfiguration);
        configure?.Invoke(obj: hostedConfiguration);

        services.AddData(configuration: hostedConfiguration.CoreData);
        services.AddEventingHostedServices(
            configuration: hostedConfiguration.Eventing);
        cCoder.Logging.IServiceCollectionExtensions
            .AddLoggingHostedServices(
                services: services,
                configuration: hostedConfiguration.Logging);

        return services;
    }
}