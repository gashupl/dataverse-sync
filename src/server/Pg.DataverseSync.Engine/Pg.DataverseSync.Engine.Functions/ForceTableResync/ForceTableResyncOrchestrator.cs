using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncOrchestrator
{
    [Function(nameof(ForceTableResyncOrchestrator))]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger<ForceTableResyncOrchestrator>();
        var tableName = context.GetInput<string>()!;

        logger.LogInformation("Resync orchestration started for table {TableName}", tableName);

        await context.CallActivityAsync(nameof(ForceTableResyncActivity), tableName);

        logger.LogInformation("Resync orchestration completed for table {TableName}", tableName);
    }
}
