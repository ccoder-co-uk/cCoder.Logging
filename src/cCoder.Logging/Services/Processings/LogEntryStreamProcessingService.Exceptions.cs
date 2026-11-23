// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Logging.Models.Exceptions;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryStreamProcessingService
{
    private static async ValueTask<T> TryCatch<T>(Func<ValueTask<T>> operation)
    {
        try
        {
            return await operation();
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