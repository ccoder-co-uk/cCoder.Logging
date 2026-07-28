// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.Exceptions;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogHubProcessingService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (LoggingValidationException innerException)
        {
            throw new LoggingValidationException(
                innerException: innerException);
        }
        catch (LoggingDependencyException innerException)
        {
            throw new LoggingDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new LoggingServiceException(
                innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(
        Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (LoggingValidationException innerException)
        {
            throw new LoggingValidationException(
                innerException: innerException);
        }
        catch (LoggingDependencyException innerException)
        {
            throw new LoggingDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new LoggingServiceException(
                innerException: innerException);
        }
    }
}