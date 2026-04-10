using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Logging.Api.OData;

internal class LoggingModelBuilder : ODataModelBuilder
{
    public LoggingModelBuilder(ODataConventionModelBuilder builder = null)
        : base(builder)
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
        return base.Builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        AddSet<LogEntry, int>();
        AddSet<LogDataItem, int>();
        base.Builder.Namespace = "";
    }
}
