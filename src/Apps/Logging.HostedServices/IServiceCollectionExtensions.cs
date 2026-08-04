// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using Logging.HostedServices.Models;

namespace Logging.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<LoggingHostedServicesConfiguration> configure = null)
    {
        LoggingHostedServicesConfiguration hostedConfiguration = new()
        {
            Logging = new LoggingConfiguration(),
            Eventing = new EventingConfiguration()
        };
        configuration.Bind(instance: hostedConfiguration);
        configure?.Invoke(obj: hostedConfiguration);

        services.AddEventingHostedServices(
            configuration: hostedConfiguration.Eventing);
        cCoder.Logging.IServiceCollectionExtensions
            .AddLoggingHostedServices(
                services: services,
                configuration: hostedConfiguration.Logging);

        return services;
    }
}