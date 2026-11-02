using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncStatusFunction : LoggingServiceBase<ForceTableResyncStatusFunction>
{
    public ForceTableResyncStatusFunction(ILogger<ForceTableResyncStatusFunction> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
    }

    [Function(nameof(ForceTableResyncStatusFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tables/{tableName}/resync/status")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        string tableName)
    {
        var instanceId = ForceTableResyncInstanceId.For(tableName);
        var existing = await client.GetInstanceAsync(instanceId);

        var isRunning = existing is { RuntimeStatus: OrchestrationRuntimeStatus.Running or OrchestrationRuntimeStatus.Pending or OrchestrationRuntimeStatus.Suspended };

        LogIfEnabled(LogLevel.Information, "Resync status for table {TableName} requested. IsRunning: {IsRunning}", tableName, isRunning);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            tableName,
            isRunning,
            status = existing?.RuntimeStatus.ToString() ?? "NotStarted"
        });
        return response;
    }
}
