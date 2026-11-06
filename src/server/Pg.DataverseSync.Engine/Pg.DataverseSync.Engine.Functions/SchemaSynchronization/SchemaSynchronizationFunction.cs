using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Functions.SchemaSynchronization.SchemaSynchronization;

public class SchemaSynchronizationFunction : LoggingServiceBase<SchemaSynchronizationFunction>
{
    private readonly ISyncMetadataService _syncMetadataService;
    private readonly IDataLoadService _dataLoadService;

    public SchemaSynchronizationFunction(
        ISyncMetadataService syncMetadataService,
        IDataLoadService dataLoadService,
        ILogger<SchemaSynchronizationFunction> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(syncMetadataService);
        ArgumentNullException.ThrowIfNull(dataLoadService);
        ArgumentNullException.ThrowIfNull(logger);

        _syncMetadataService = syncMetadataService;
        _dataLoadService = dataLoadService;
    }

    [Function(nameof(SchemaSynchronizationFunction))]
    public void Run(
//#if DEBUG
//      [TimerTrigger("%SchemaSyncSchedule%", RunOnStartup = true)] TimerInfo timer)
//#else
        [TimerTrigger("%SchemaSyncSchedule%")] TimerInfo timer)
//#endif
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

            if (table.OperationCode == TableSyncOperationCode.Create)
            {
                LogIfEnabled(LogLevel.Information,
                    "Table {TableName} was created in the target database. Performing initial data load...",
                    table.TableName);

                _dataLoadService.LoadInitialData(table.TableName);
            }
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
