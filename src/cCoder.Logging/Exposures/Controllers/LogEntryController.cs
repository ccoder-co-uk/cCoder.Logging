using cCoder.Logging.Api.OData;
using cCoder.Logging.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Logging.Exposures.Controllers;

public partial class LogEntryController : ODataController
{
    protected ILogEntryOrchestrationService Service { get; }

    public LogEntryController(
        ILogEntryOrchestrationService service,
        ILogger<LogEntryController> log
    ) => Service = service;

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new cCoder.Logging.Api.OData.LoggingModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(LogEntry))
            )
            : Ok(new MetadataContainer(typeof(LogEntry), true, true));
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public IActionResult Get(ODataQueryOptions<LogEntry> queryOptions) => Ok(Service.GetAll());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] int key)
    {
        IQueryable<LogEntry> result = Service.GetAll().AsQueryable().Where(logEntry => logEntry.Id == key);
        return Ok(SingleResult.Create(result));
    }

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] LogEntry entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.Logging.Api.OData.BadRequestResult(ModelState);

        return Ok(await Service.AddAsync(entity));
    }

}



