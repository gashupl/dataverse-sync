using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public class ForceTableResyncActivity : LoggingServiceBase<ForceTableResyncActivity>
{

    private readonly ISyncMetadataService _syncMetadataService;
    private readonly IDataLoadService _dataLoadService;

    public ForceTableResyncActivity(
        ISyncMetadataService syncMetadataService,
        IDataLoadService dataLoadService, ILogger<ForceTableResyncActivity> logger, TimeSpan? delay = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(syncMetadataService);
        ArgumentNullException.ThrowIfNull(dataLoadService);
        ArgumentNullException.ThrowIfNull(logger);

        _syncMetadataService = syncMetadataService;
        _dataLoadService = dataLoadService;
    }

    [Function(nameof(ForceTableResyncActivity))]
    public async Task Run([ActivityTrigger] string tableName)
    {
        LogIfEnabled(LogLevel.Information, "Resync activity started for table {TableName}", tableName);

        var result = _syncMetadataService.Execute(tableName);

        if (result?.TablesSyncResult == null)
        {
            LogIfEnabled(LogLevel.Error, "Schema synchronization failed. Result is null.");
            return;
        }

        var succeeded = result.TablesSyncResult.Where(t => t.IsSynchronized).ToList();

        if (succeeded.Count == 1)
        {
            LogIfEnabled(LogLevel.Information, "Table {TableName} resynchronized successfully.", tableName);

            if (succeeded[0].OperationCode == TableSyncOperationCode.Create)
            {
                LogIfEnabled(LogLevel.Information,
                    "Table {TableName} was created in the target database. Performing initial data load...",
                    tableName);

                _dataLoadService.LoadInitialData(tableName);
            }
        }
        else
        {
            var errorMessage = result.TablesSyncResult.FirstOrDefault(t => !t.IsSynchronized)?.ErrorMessage;
            LogIfEnabled(LogLevel.Error, "Table {TableName} resynchronization failed. Error: {ErrorMessage}",
                tableName, errorMessage ?? "Unknown error.");
        }

        LogIfEnabled(LogLevel.Information, "Resync activity completed for table {TableName}", tableName);
    }
}
