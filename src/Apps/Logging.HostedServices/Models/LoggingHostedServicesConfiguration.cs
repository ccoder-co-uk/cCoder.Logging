// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;

namespace Logging.HostedServices.Models;

public sealed class LoggingHostedServicesConfiguration
{
    public LoggingHostedServicesConfiguration()
    {
        Logging = new LoggingConfiguration();
        Data = new DataConfiguration();
        Eventing = new EventingConfiguration();
    }

    public LoggingConfiguration Logging { get; set; }
    public DataConfiguration Data { get; set; }
    public EventingConfiguration Eventing { get; set; }
}