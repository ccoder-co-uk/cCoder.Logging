// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Models.OData;
using cCoder.Logging.Models;
using cCoder.Logging.Models.Exceptions;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Exposures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Logging.Exposures.Controllers;

public partial class LogDataItemController(
    ILogDataItemManager logDataItemManager)
        : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query["extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(
                    value: new LoggingModelBroker()
                        .Build()
                        .EDMModel.GetExtendedMetadataForType(
                            context: "Logging",
                            type: typeof(LogDataItem))
                )
                : Ok(
                    value: PropertyInfoExtensions.CreateMetadataContainer(
                        type: typeof(LogDataItem),
                        isEntity: true,
                        hasEndpoint: true));
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log data item metadata request failed.");
        }
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
    public IActionResult Get()
    {
        try
        {
            return Ok(value: logDataItemManager.GetAllLogDataItems());
        }
        catch (LoggingValidationException)
        {
            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log data item request failed.");
        }
    }

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
        try
        {
            LogDataItem logDataItem = logDataItemManager.GetLogDataItem(
                logDataItemId: key);

            if (logDataItem is null)
            {
                return NotFound();
            }

            return Ok(value: logDataItem);
        }
        catch (LoggingValidationException)
        {
            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log data item request failed.");
        }
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
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            LogDataItem savedLogDataItem = await logDataItemManager.AddLogDataItemAsync(
                newLogDataItem: newLogDataItem);

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: savedLogDataItem);
        }
        catch (LoggingValidationException)
        {
            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log data item request failed.");
        }
    }
}