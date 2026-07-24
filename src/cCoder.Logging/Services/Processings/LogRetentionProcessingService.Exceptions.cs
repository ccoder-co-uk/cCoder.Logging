// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.Exceptions;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryRetentionProcessingService
{
    private static async Task TryCatch(Func<Task> operation)
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

    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
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