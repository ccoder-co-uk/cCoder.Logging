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

public partial class LogEntryController(
    ILogEntryOrchestrationService logEntryOrchestrationService,
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
            IQueryable<LogEntry> logEntries =
                logEntryOrchestrationService.GetAllLogEntries();

            loggingBroker.LogInformation(
                message: "Log entries retrieved successfully.");

            return Ok(value: logEntries);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was invalid.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was forbidden.");

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
            LogEntry logEntry = logEntryOrchestrationService.GetLogEntry(
                logEntryId: key);

            if (logEntry is null)
            {
                loggingBroker.LogWarning(
                    message: "The requested log entry was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(
                message: "Log entry retrieved successfully.");

            return Ok(value: logEntry);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was invalid.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was forbidden.");

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
                loggingBroker.LogWarning(
                    message: "The log entry request was invalid.");

                return BadRequest(modelState: ModelState);
            }

            LogEntry savedLogEntry = await logEntryOrchestrationService.AddLogEntryAsync(
                newLogEntry: newLogEntry);

            loggingBroker.LogInformation(
                message: "Log entry created successfully.");

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: savedLogEntry);
        }
        catch (LoggingValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was invalid.");

            return BadRequest(error: "The log entry request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The log entry request was forbidden.");

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