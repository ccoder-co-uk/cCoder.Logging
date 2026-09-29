// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogEntryStreamProcessingService
{
    ValueTask StreamLogEntryCaptureRequestAsync(
        LogEntryCaptureRequest logEntryCaptureRequest);
}