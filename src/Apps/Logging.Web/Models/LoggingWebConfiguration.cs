// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using cCoder.Security.Objects;

namespace Logging.Web.Models;

public sealed class LoggingWebConfiguration
{
    public LoggingWebConfiguration()
    {
        Logging = new LoggingConfiguration();
        Data = new DataConfiguration();
        Security = new SecurityConfiguration();
        Eventing = new EventingConfiguration();
    }

    public LoggingConfiguration Logging { get; set; }
    public DataConfiguration Data { get; set; }
    public SecurityConfiguration Security { get; set; }
    public EventingConfiguration Eventing { get; set; }
}