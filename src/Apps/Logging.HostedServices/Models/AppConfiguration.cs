// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;

namespace Logging.HostedServices.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        CoreData = new CoreDataConfiguration();
        Eventing = new EventingConfiguration();
        Logging = new LoggingConfiguration();
    }

    public CoreDataConfiguration CoreData { get; set; }

    public EventingConfiguration Eventing { get; set; }

    public LoggingConfiguration Logging { get; set; }
}