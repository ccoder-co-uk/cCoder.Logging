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
    : ILoggingModelBroker
{
    private readonly ODataConventionModelBuilder builder;

    public LoggingModelBroker(ODataConventionModelBuilder builder = null)
    {
        this.builder = builder ?? new ODataConventionModelBuilder();
    }

    public ODataModel Build()
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
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();
        builder.EntitySet<LogEntry>(name: nameof(LogEntry));
        builder.EntitySet<LogDataItem>(name: nameof(LogDataItem));
        builder.Namespace = "";
    }
}