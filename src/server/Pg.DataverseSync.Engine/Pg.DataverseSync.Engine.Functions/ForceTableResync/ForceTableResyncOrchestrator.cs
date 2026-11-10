using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Target.SqlServer;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncOrchestrator : LoggingServiceBase<ForceTableResyncOrchestrator>
{
    public ForceTableResyncOrchestrator(ILogger<ForceTableResyncOrchestrator> logger) : base(logger)
    {
    }

    [Function(nameof(ForceTableResyncOrchestrator))]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var tableName = context.GetInput<string>()!;

        LogIfEnabled(LogLevel.Information, "Resync orchestration started for table {TableName}", tableName);

        await context.CallActivityAsync(nameof(ForceTableResyncActivity), tableName);

        LogIfEnabled(LogLevel.Information, "Resync orchestration completed for table {TableName}", tableName);
    }
}
