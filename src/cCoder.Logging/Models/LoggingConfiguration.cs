using cCoder.Eventing.Models;

namespace cCoder.Logging.Models;

public class LoggingConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public bool StoreLogEntries { get; set; }
    public bool StreamLogEntries { get; set; } = true;
    public int RetentionDays { get; set; } = 30;
    public int RetentionIntervalMinutes { get; set; } = 60;
    public int? DefaultAppId { get; set; }
    public string DefaultAppDomain { get; set; }
    public string RootPath { get; set; } = "Api/Logging";
    public bool IncludeLegacyCoreContext { get; set; } = true;
    public EventProvider[] EventProviders { get; private set; } = [];

    public LoggingConfiguration WithEventProviders(params EventProvider[] eventProviders)
    {
        EventProviders = eventProviders ?? [];
        return this;
    }
}
