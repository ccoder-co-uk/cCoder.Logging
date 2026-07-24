// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Models.Exceptions;

public sealed class LoggingValidationException(Exception innerException)
    : Exception(
        message: "Logging validation failed.",
        innerException: innerException);