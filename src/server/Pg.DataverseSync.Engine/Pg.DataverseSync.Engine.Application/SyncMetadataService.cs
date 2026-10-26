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
                    if (!sourceTableMap.TryGetValue(synchronizedTableName, out var sourceTable))
                    {
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None, 
                            "Table is missing in source metadata."));
                        continue;
                    }

                    if (!context.TargetTableExists.ContainsKey(synchronizedTableName))
                    {
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None,
                            "Target table existence could not be determined."));
                        continue;
                    }

                    var currentChecksum = sourceTable.GetChecksum();
                    var savedChecksum = _dataRepository
                        .GetSyncDetails(synchronizedTableName, _targetRepositorySet)
                        .FirstOrDefault()?.pg_checksum;  

                    if(currentChecksum == savedChecksum)
                    {
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, true, TableSyncOperationCode.None));
                        LogIfEnabled(LogLevel.Information, $"Metadata synchronization for {synchronizedTableName} table is not required. Consistent checksums found.",
                            synchronizedTableName);
                        continue;
                    }

                    try
                    {
                        var modificationResult = _targetSchemaService.UpsertTargetTable(sourceTable);
                        if (modificationResult.Success != SchemaModificationResult.Success)
                        {
                            result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false,
                                TableSyncOperationCode.None, modificationResult.Message));
                            continue;
                        }

                        var syncDetailsModificationResult =_sourceMetadataService.UpsertSyncDetails(synchronizedTableName, currentChecksum, _targetRepositorySet);        
                        if(syncDetailsModificationResult.Success != ModificationResult.Success)
                        {
                            result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false,
                                TableSyncOperationCode.None, syncDetailsModificationResult.Message));
                            continue;
                        }

                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, true, modificationResult.OperationCode));
                    }
                    catch (Exception ex)
                    {
                        LogIfEnabled(LogLevel.Error, ex,
                            "Synchronization failed for table {TableName}.", synchronizedTableName);
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false, TableSyncOperationCode.None, ex.Message));
                    }
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
    }
}
