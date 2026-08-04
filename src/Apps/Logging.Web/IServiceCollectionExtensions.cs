// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using cCoder.Security;
using cCoder.Security.Models;
using Logging.Web.Models;

namespace Logging.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<LoggingWebConfiguration> configure = null)
    {
        LoggingWebConfiguration webConfiguration = new()
        {
            Logging = new LoggingConfiguration(),
            Security = new SecurityConfiguration(),
            Eventing = new EventingConfiguration()
        };
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