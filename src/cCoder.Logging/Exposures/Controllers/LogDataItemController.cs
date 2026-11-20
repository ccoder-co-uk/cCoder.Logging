// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Security.Authentication;
using System.Linq;

using System.Threading.Tasks;

using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Models.OData;
using cCoder.Logging.Models;
using cCoder.Logging.Models.Exceptions;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Logging.Exposures.Controllers;

public partial class LogDataItemController(
    ILogDataItemOrchestrationService logDataItemOrchestrationService,
    ILoggingBroker loggingBroker)
        : ODataController
{
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
            IQueryable<LogDataItem> logDataItems =
                logDataItemOrchestrationService.GetAllLogDataItems();

            loggingBroker.LogInformation(
                message: "Log data items retrieved successfully.");

            return Ok(value: logDataItems);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was invalid.");

            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was forbidden.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

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
            LogDataItem logDataItem = logDataItemOrchestrationService.GetLogDataItem(
                logDataItemId: key);

            if (logDataItem is null)
            {
                loggingBroker.LogWarning(
                    message: "The requested log data item was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(
                message: "Log data item retrieved successfully.");

            return Ok(value: logDataItem);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was invalid.");

            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was forbidden.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

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
                loggingBroker.LogWarning(
                    message: "The log data item request was invalid.");

                return BadRequest(modelState: ModelState);
            }

            LogDataItem savedLogDataItem = await logDataItemOrchestrationService.AddLogDataItemAsync(
                newLogDataItem: newLogDataItem);

            loggingBroker.LogInformation(
                message: "Log data item created successfully.");

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: savedLogDataItem);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was invalid.");

            return BadRequest(error: "The log data item request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log data item request was forbidden.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log data item request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log data item request failed.");
        }
    }
}