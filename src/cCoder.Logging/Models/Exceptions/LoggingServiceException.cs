// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Logging.Models.Exceptions;

public sealed class LoggingServiceException(Exception innerException)
    : Exception(
        message: "The logging service failed.",
        innerException: innerException);