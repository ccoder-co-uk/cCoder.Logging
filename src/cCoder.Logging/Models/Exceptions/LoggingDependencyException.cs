// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Models.Exceptions;

public sealed class LoggingDependencyException(Exception innerException)
    : Exception(
        message: "A logging dependency failed.",
        innerException: innerException);
