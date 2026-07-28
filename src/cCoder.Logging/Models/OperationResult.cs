// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Models;

internal sealed class OperationResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Item { get; set; }
}