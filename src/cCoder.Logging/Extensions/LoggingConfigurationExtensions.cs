// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Logging.Models;

namespace cCoder.Logging.Extensions;

public static class LoggingConfigurationExtensions
{
    public static LoggingConfiguration WithEventProviders(
        this LoggingConfiguration configuration,
        params EventProvider[] eventProviders)
    {
        configuration.EventProviders = eventProviders ?? [];
        return configuration;
    }
}