// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Models.Exceptions;

public sealed class LoggingServiceException(Exception innerException)
    : Exception(
        message: "The logging service failed.",
        innerException: innerException);
