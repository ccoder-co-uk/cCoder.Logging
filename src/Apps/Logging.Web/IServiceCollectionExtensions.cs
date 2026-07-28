// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
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

        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddData(configuration: webConfiguration.Data);
        services.AddSecurityWeb(configuration: webConfiguration.Security);
        cCoder.Logging.IServiceCollectionExtensions.AddLoggingWeb(
            services: services,
            configuration: webConfiguration.Logging);

        return services;
    }
}