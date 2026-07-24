// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.Exceptions;

namespace cCoder.Logging.Services.Foundations.Events;

internal sealed partial class LogDataItemEventService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
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
