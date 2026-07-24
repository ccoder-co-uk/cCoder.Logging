// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Logging.Models;

public class LoggingConfiguration
{
    public LoggingConfiguration()
    {
        ConnectionStrings = new Dictionary<string, string>();
        Settings = new Dictionary<string, string>();
        Services = new Dictionary<string, string>();
        StreamLogEntries = true;
        RetentionDays = 30;
        RetentionIntervalMinutes = 60;
        RootPath = "Api/Logging";
        IncludeLegacyCoreContext = true;
        EventProviders = [];
    }

    public IDictionary<string, string> ConnectionStrings { get; set; }
    public IDictionary<string, string> Settings { get; set; }
    public IDictionary<string, string> Services { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public bool StoreLogEntries { get; set; }
    public bool StreamLogEntries { get; set; }
    public int RetentionDays { get; set; }
    public int RetentionIntervalMinutes { get; set; }
    public int? DefaultAppId { get; set; }
    public string DefaultAppDomain { get; set; }
    public string RootPath { get; set; }
    public bool IncludeLegacyCoreContext { get; set; }
    public EventProvider[] EventProviders { get; set; }
}