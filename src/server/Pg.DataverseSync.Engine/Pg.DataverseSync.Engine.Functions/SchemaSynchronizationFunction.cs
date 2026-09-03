using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Functions;

public class SchemaSynchronizationFunction : LoggingServiceBase<SchemaSynchronizationFunction>
{
    private readonly ISyncMetadataService _syncMetadataService;

    public SchemaSynchronizationFunction(
        ISyncMetadataService syncMetadataService,
        ILogger<SchemaSynchronizationFunction> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(syncMetadataService);
        ArgumentNullException.ThrowIfNull(logger);

        _syncMetadataService = syncMetadataService;
    }

    [Function(nameof(SchemaSynchronizationFunction))]
    public void Run(
#if DEBUG
        [TimerTrigger("%SchemaSyncSchedule%", RunOnStartup = true)] TimerInfo timer)
#else
        [TimerTrigger("%SchemaSyncSchedule%")] TimerInfo timer)
#endif
    {
        LogIfEnabled(LogLevel.Information, "SchemaSynchronizationFunction triggered at: {UtcNow}", DateTime.UtcNow);

        var result = _syncMetadataService.Execute();

        if (result?.TablesSyncResult == null) 
        {
            LogIfEnabled(LogLevel.Error, "Schema synchronization failed. Result is null.");
            return;
        }

        var succeeded = result.TablesSyncResult.Where(t => t.IsSynchronized).ToList();
        var failed = result.TablesSyncResult.Where(t => !t.IsSynchronized).ToList();

        foreach (var table in succeeded)
        {
            LogIfEnabled(LogLevel.Information, "Table {TableName} synchronized successfully.", table.TableName);
        }

        foreach (var table in failed)
        {
            LogIfEnabled(LogLevel.Error, "Table {TableName} synchronization failed. Error: {ErrorMessage}",
                table.TableName, table.ErrorMessage);
        }

        LogIfEnabled(LogLevel.Information,
            "Schema synchronization completed. Succeeded: {SucceededCount}, Failed: {FailedCount}.",
            succeeded.Count, failed.Count);
    }
}
