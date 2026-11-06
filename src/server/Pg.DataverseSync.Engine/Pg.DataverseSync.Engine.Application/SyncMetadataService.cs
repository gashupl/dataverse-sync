using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Application.Synchronization;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Target;
using Pg.DataverseSync.Engine.Core.Schema;
using Pg.DataverseSync.Engine.Application.Data;

namespace Pg.DataverseSync.Engine.Application
{
    public class SyncMetadataService : LoggingServiceBase<SyncMetadataService>, ISyncMetadataService
    {
        private readonly ISourceMetadataService _sourceMetadataService;
        private readonly ITargetSchemaService _targetSchemaService;
        private readonly IDataRepository _dataRepository;

        private readonly pg_TargetRepositorySet _targetRepositorySet = pg_TargetRepositorySet.MicrosoftSqlServer; //TODO: Make this configurable

        public SyncMetadataService(ISourceMetadataService sourceMetadataService,
            ITargetSchemaService targetSchemaService,
            IDataRepository dataRepository,
            ILogger<SyncMetadataService> logger) : base(logger)
        {
            _sourceMetadataService = sourceMetadataService;
            _targetSchemaService = targetSchemaService;
            _dataRepository = dataRepository;
        }

        public SyncMetadataResult Execute()
        {
            LogIfEnabled(LogLevel.Information, "Executing metadata synchronization...");

            try
            {
                var context = new SyncMetadataExecutionContext();

                var retrieveSynchronizedTablesHandler = new RetrieveSynchronizedTablesHandler(_sourceMetadataService);
                var retrieveSourceTablesHandler = new RetrieveSourceTablesHandler(_sourceMetadataService);
                var checkTargetSchemaHandler = new CheckTargetSchemaHandler(_targetSchemaService);

                retrieveSynchronizedTablesHandler
                    .SetNext(retrieveSourceTablesHandler)
                    .SetNext(checkTargetSchemaHandler);

                retrieveSynchronizedTablesHandler.Handle(context);

                var sourceTables = context.SourceTables ?? [];
                var sourceTableMap = sourceTables.ToDictionary(table => table.Name, StringComparer.OrdinalIgnoreCase);

                var result = new SyncMetadataResult();
                foreach (var synchronizedTableName in context.SynchronizedTableNames)
                {
                    sourceTableMap.TryGetValue(synchronizedTableName, out var sourceTable);
                    var targetExistenceKnown = context.TargetTableExists.ContainsKey(synchronizedTableName);

                    var syncResult = SynchronizeTable(synchronizedTableName, sourceTable, targetExistenceKnown); 
                    result.TablesSyncResult.Add(syncResult);
                }

                LogIfEnabled(LogLevel.Information, "Metadata synchronization completed for {SyncedTableCount} table(s).",
                    result.TablesSyncResult.Count);

                return result;
            }
            catch (ApplicationServiceException ex)
            {
                LogIfEnabled(LogLevel.Error, ex, "Metadata synchronization failed.");
                throw;
            }
            catch (Exception ex)
            {
                const string message = "An error occurred while executing metadata synchronization.";
                LogIfEnabled(LogLevel.Error, ex, message);
                throw new ApplicationServiceException(message, ex);
            }
        }

        public SyncMetadataResult Execute(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            LogIfEnabled(LogLevel.Information, "Executing metadata synchronization for table {TableName}...", tableName);

            try
            {
                Table? sourceTable;

                try
                {
                    sourceTable = (_sourceMetadataService.GetTables([tableName]) ?? [])
                        .FirstOrDefault(table => string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase));
                }
                catch (Exception ex)
                {
                    throw new ApplicationServiceException(
                        "An error occurred while retrieving source tables metadata.", ex);
                }

                var result = new SyncMetadataResult();
                result.TablesSyncResult.Add(SynchronizeTable(tableName, sourceTable, true));

                LogIfEnabled(LogLevel.Information, "Metadata synchronization completed for table {TableName}.", tableName);

                return result;
            }
            catch (ApplicationServiceException ex)
            {
                LogIfEnabled(LogLevel.Error, ex, "Metadata synchronization failed for table {TableName}.", tableName);
                throw;
            }
            catch (Exception ex)
            {
                const string message = "An error occurred while executing metadata synchronization.";
                LogIfEnabled(LogLevel.Error, ex, message);
                throw new ApplicationServiceException(message, ex);
            }
        }
        public PurgeTableResult PurgeTable(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            LogIfEnabled(LogLevel.Information, "Purging table {TableName}...", tableName);

            try
            {
                if (_targetSchemaService.TargetTableExists(tableName))
                {
                    var targetDeleteResult = _targetSchemaService.DeleteTargetTable(tableName);
                    if (targetDeleteResult.Success != SchemaModificationResult.Success)
                    {
                        var errorMessage = targetDeleteResult.Message ?? "Failed to delete table from target database.";
                        LogIfEnabled(LogLevel.Warning, "Purge failed for table {TableName}. {ErrorMessage}", tableName, errorMessage);
                        return new PurgeTableResult(tableName, false, errorMessage);
                    }
                }

                var syncDetails = _dataRepository.GetSyncDetails(tableName, _targetRepositorySet);
                foreach (var syncDetail in syncDetails)
                {
                    _dataRepository.DeleteRecord(syncDetail.Id, pg_synchronizationdetails.EntityLogicalName);
                }

                LogIfEnabled(LogLevel.Information,
                    "Purge completed for table {TableName}. Removed {SyncDetailsCount} synchronization detail record(s).",
                    tableName,
                    syncDetails.Count);

                return new PurgeTableResult(tableName, true);
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, ex, "Purge failed for table {TableName}.", tableName);
                return new PurgeTableResult(tableName, false, ex.Message);
            }
        }

        private TableSyncResult SynchronizeTable(string synchronizedTableName, Table? sourceTable, bool targetExistenceKnown)
        {
            if (sourceTable == null)
            {
                return new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None,
                    "Table is missing in source metadata.");
            }

            if (!targetExistenceKnown)
            {
                return new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None,
                    "Target table existence could not be determined.");
            }

            var currentChecksum = sourceTable.GetChecksum();
            var savedChecksum = _dataRepository
                .GetSyncDetails(synchronizedTableName, _targetRepositorySet)
                .FirstOrDefault()?.pg_checksum;

            if (currentChecksum == savedChecksum)
            {
                LogIfEnabled(LogLevel.Information, $"Metadata synchronization for {synchronizedTableName} table is not required. Consistent checksums found.",
                    synchronizedTableName);
                return new TableSyncResult(synchronizedTableName, true, TableSyncOperationCode.None);
            }

            try
            {
                var modificationResult = _targetSchemaService.UpsertTargetTable(sourceTable);
                if (modificationResult.Success != SchemaModificationResult.Success)
                {
                    return new TableSyncResult(synchronizedTableName, false,
                        TableSyncOperationCode.None, modificationResult.Message);
                }

                var syncDetailsModificationResult = _sourceMetadataService.UpsertSyncDetails(synchronizedTableName, currentChecksum, _targetRepositorySet);
                if (syncDetailsModificationResult.Success != ModificationResult.Success)
                {
                    return new TableSyncResult(synchronizedTableName, false,
                        TableSyncOperationCode.None, syncDetailsModificationResult.Message);
                }

                return new TableSyncResult(synchronizedTableName, true, modificationResult.OperationCode);
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, ex,
                    "Synchronization failed for table {TableName}.", synchronizedTableName);
                return new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None, ex.Message);
            }
        }
    }
}
