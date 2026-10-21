// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;

namespace cCoder.Logging;

[System.CodeDom.Compiler.GeneratedCode("cCoder.Logging", "1.0")]
internal sealed class RequestLogQueueCoordinator(
    ILogEntryCaptureQueue queue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<RequestLogQueueCoordinator> logger)
        : IRequestLogQueueCoordinator
{
    public async Task RunAsync()
    {
        await foreach (LogEntryCaptureRequest request in queue.ReadAllAsync())
        {
            await PersistAsync(request: request);
        }
    }

    public void Complete() => queue.Complete();

    private async Task PersistAsync(LogEntryCaptureRequest request)
    {
        try
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            ILogEntryCaptureOrchestrationService captureService =
                scope.ServiceProvider.GetRequiredService<ILogEntryCaptureOrchestrationService>();

            await captureService.CaptureLogEntryCaptureRequestAsync(
                logEntryCaptureRequest: request);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception: exception,
                message: "Unable to persist queued request log entry.");
        }
    }
}