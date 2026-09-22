// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.Exceptions;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryStreamService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ArgumentException innerException)
        {
            throw new LoggingValidationException(innerException: innerException);
        }
        catch (LoggingDependencyException innerException)
        {
            throw new LoggingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new LoggingServiceException(innerException: innerException);
        }
    }
}