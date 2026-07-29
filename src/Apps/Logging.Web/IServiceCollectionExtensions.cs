// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Security;
using Logging.Web.Models;

namespace Logging.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<LoggingWebConfiguration> configure = null)
    {
        LoggingWebConfiguration webConfiguration = new();
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        cCoder.Logging.IServiceCollectionExtensions.AddLoggingWeb(
            services: services,
            configuration: webConfiguration.Logging);
        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddSecurityWeb(configuration: webConfiguration.Security);

        return services;
    }
}