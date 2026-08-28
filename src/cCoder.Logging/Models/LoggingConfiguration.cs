// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Logging.Models;

public class LoggingConfiguration
{
    public bool StoreLogEntries { get; set; }
    public bool StreamLogEntries { get; set; }
    public int RetentionDays { get; set; }
    public int RetentionIntervalMinutes { get; set; }
    public int? DefaultAppId { get; set; }
    public string DefaultAppDomain { get; set; }
    public string RootPath { get; set; }
    public bool RequestLoggingEnabled { get; set; }
    public int RequestLoggingQueueCapacity { get; set; }
    public RequestLoggingQueueFullBehavior RequestLoggingQueueFullBehavior { get; set; }
    public LogLevel DatabaseMinimumLogLevel { get; set; }
    public EventProvider[] EventProviders { get; set; }
}