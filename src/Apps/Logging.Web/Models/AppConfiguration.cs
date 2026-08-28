// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using cCoder.Security.Models;

namespace Logging.Web.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        CoreData = new CoreDataConfiguration();
        Eventing = new EventingConfiguration();
        Logging = new LoggingConfiguration();
        Security = new SecurityConfiguration();
        SecurityData = new SecurityDataConfiguration();
    }

    public CoreDataConfiguration CoreData { get; set; }

    public EventingConfiguration Eventing { get; set; }

    public LoggingConfiguration Logging { get; set; }

    public SecurityConfiguration Security { get; set; }

    public SecurityDataConfiguration SecurityData { get; set; }
}