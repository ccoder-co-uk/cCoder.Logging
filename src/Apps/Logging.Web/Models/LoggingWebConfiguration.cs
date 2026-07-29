// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using cCoder.Security.Models;

namespace Logging.Web.Models;

public sealed class LoggingWebConfiguration
{
    public LoggingWebConfiguration()
    {
        Logging = new LoggingConfiguration();
        Security = new SecurityConfiguration();
        Eventing = new EventingConfiguration();
    }

    public LoggingConfiguration Logging { get; set; }
    public SecurityConfiguration Security { get; set; }
    public EventingConfiguration Eventing { get; set; }
}