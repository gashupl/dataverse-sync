using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncHttpStarter : LoggingServiceBase<ForceTableResyncHttpStarter>
{
    public ForceTableResyncHttpStarter(ILogger<ForceTableResyncHttpStarter> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
    }

    [Function(nameof(ForceTableResyncHttpStarter))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tables/{tableName}/resync")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            LogIfEnabled(LogLevel.Warning, "Resync request rejected. Required parameter {ParameterName} is missing.", nameof(tableName));

            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync($"Missing required parameter '{nameof(tableName)}'.");
            return badRequest;
        }

        var instanceId = ForceTableResyncInstanceId.For(tableName);
        var existing = await client.GetInstanceAsync(instanceId);

        if (existing is { RuntimeStatus: OrchestrationRuntimeStatus.Running or OrchestrationRuntimeStatus.Pending or OrchestrationRuntimeStatus.Suspended })
        {
            LogIfEnabled(LogLevel.Warning, "Resync for table {TableName} is already running. InstanceId: {InstanceId}", tableName, instanceId);

            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            await conflict.WriteStringAsync($"Resync for table '{tableName}' is already running.");
            return conflict;
        }

        await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ForceTableResyncOrchestrator),
            tableName,
            new StartOrchestrationOptions(instanceId));

        LogIfEnabled(LogLevel.Information, "Resync for table {TableName} started. InstanceId: {InstanceId}", tableName, instanceId);

        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}
