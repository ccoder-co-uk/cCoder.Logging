// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using Logging.HostedServices.Models;

namespace Logging.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<LoggingHostedServicesConfiguration> configure = null)
    {
        LoggingHostedServicesConfiguration hostedConfiguration = new();
        configuration.Bind(instance: hostedConfiguration);
        configure?.Invoke(obj: hostedConfiguration);

        services.AddEventingHostedServices(
            configuration: hostedConfiguration.Eventing);
        services.AddData(configuration: hostedConfiguration.Data);
        cCoder.Logging.IServiceCollectionExtensions
            .AddLoggingHostedServices(
                services: services,
                configuration: hostedConfiguration.Logging);

        return services;
    }
}