// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Api.OData;
using cCoder.Logging.Dependencies.OData;
using cCoder.Logging.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Exposures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Logging.Exposures.Controllers;

public partial class LogDataItemController(
    ILogDataItemManager logDataItemManager)
        : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new LoggingModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Logging", typeof(LogDataItem))
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
    public IActionResult Get(ODataQueryOptions<LogDataItem> queryOptions) =>
        Ok(value: logDataItemManager.GetAllLogDataItems());

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
        IQueryable<LogDataItem> result = logDataItemManager
            .GetAllLogDataItems()
            .AsQueryable()
            .Where(predicate: logDataItem => logDataItem.Id == key);

        return Ok(value: SingleResult.Create(queryable: result));
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
    public async Task<IActionResult> Post([FromBody] LogDataItem newLogDataItem)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Logging.Api.OData.BadRequestResult(ModelState);
        }

        LogDataItem savedLogDataItem = await logDataItemManager.AddLogDataItemAsync(
            newLogDataItem: newLogDataItem);

        return Ok(value: savedLogDataItem);
    }
}
