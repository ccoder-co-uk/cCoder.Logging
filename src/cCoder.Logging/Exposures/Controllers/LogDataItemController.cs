using cCoder.Logging.Api.OData;
using cCoder.Logging.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Logging.Exposures.Controllers;

public partial class LogDataItemController : ODataController
{
    protected ILogDataItemOrchestrationService Service { get; }

    public LogDataItemController(
        ILogDataItemOrchestrationService service,
        ILogger<LogDataItemController> log
    ) => Service = service;

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new cCoder.Logging.Api.OData.LoggingModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(LogDataItem))
            )
            : Ok(new MetadataContainer(typeof(LogDataItem), true, true));
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
    public IActionResult Get(ODataQueryOptions<LogDataItem> queryOptions) => Ok(Service.GetAll());

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
        IQueryable<LogDataItem> result = Service.GetAll().AsQueryable().Where(logDataItem => logDataItem.Id == key);
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
    public async Task<IActionResult> Post([FromBody] LogDataItem entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.Logging.Api.OData.BadRequestResult(ModelState);

        return Ok(await Service.AddAsync(entity));
    }

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] LogDataItem entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.Logging.Api.OData.BadRequestResult(ModelState);

        return Ok(await Service.UpdateAsync(entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<LogDataItem> delta)
    {
        LogDataItem originalEntity = Service.Get(key);
        if (originalEntity == null)
            return NotFound();

        delta.Patch(originalEntity);
        return Ok(await Service.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }
}



