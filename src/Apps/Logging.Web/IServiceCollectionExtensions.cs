// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Security;
using cCoder.Security.Data.EF;
using Logging.Web.Models;

namespace Logging.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration webConfiguration = new();
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        services.AddData(configuration: webConfiguration.CoreData);
        services.AddSecurityData(configuration: webConfiguration.SecurityData);
        cCoder.Logging.IServiceCollectionExtensions.AddLoggingWeb(
            services: services,
            configuration: webConfiguration.Logging);
        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddSecurityWeb(configuration: webConfiguration.Security);

        return services;
    }
}