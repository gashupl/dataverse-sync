using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncActivity : LoggingServiceBase<ForceTableResyncActivity>
{
    public ForceTableResyncActivity(ILogger<ForceTableResyncActivity> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
    }

    [Function(nameof(ForceTableResyncActivity))]
    public Task Run([ActivityTrigger] string tableName)
    {
        LogIfEnabled(LogLevel.Information, "Resync activity executed for table {TableName}", tableName);
        return Task.CompletedTask;
    }
}
