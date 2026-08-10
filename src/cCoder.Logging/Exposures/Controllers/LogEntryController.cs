// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers.Loggings;
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

public partial class LogEntryController(
    ILogEntryManager logEntryManager,
    ILoggingBroker loggingBroker)
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
                            type: typeof(LogEntry))
                )
                : Ok(
                    value: PropertyInfoExtensions.CreateMetadataContainer(
                        type: typeof(LogEntry),
                        isEntity: true,
                        hasEndpoint: true));
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log entry metadata request failed.");
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
            return Ok(value: logEntryManager.GetAllLogEntries());
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log entry request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log entry request failed.");
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
            LogEntry logEntry = logEntryManager.GetLogEntry(
                logEntryId: key);

            if (logEntry is null)
            {
                return NotFound();
            }

            return Ok(value: logEntry);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log entry request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log entry request failed.");
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
    public async Task<IActionResult> Post([FromBody] LogEntry newLogEntry)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            LogEntry savedLogEntry = await logEntryManager.AddLogEntryAsync(
                newLogEntry: newLogEntry);

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: savedLogEntry);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The log entry request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The log entry request failed.");
        }
    }
}