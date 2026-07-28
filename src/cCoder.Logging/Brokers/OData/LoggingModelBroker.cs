// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Logging.Models.OData;
using cCoder.Data.Models.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Logging.Brokers.OData;

internal class LoggingModelBroker
    : ODataModelBroker,
      ILoggingModelBroker
{
    public LoggingModelBroker(ODataConventionModelBuilder builder = null)
        : base(builder: builder)
    {
    }

    public override ODataModel Build()
    {
        return new ODataModel
        {
            Context = "Core",
            Description = "Logging endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };
    }

    public void Configure()
    {
        ConfigureModel();
    }

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        AddSet<LogEntry, int>();
        AddSet<LogDataItem, int>();
        builder.Namespace = "";
    }
}