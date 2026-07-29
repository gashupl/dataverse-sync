using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Application.Synchronization;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application
{
    public class SyncMetadataService : LoggingServiceBase<SyncMetadataService>, ISyncMetadataService
    {
        private readonly ISourceMetadataService _sourceMetadataService;
        private readonly ITargetSchemaService _targetSchemaService;

        public SyncMetadataService(ISourceMetadataService sourceMetadataService,
            ITargetSchemaService targetSchemaService,
            ILogger<SyncMetadataService> logger) : base(logger)
        {
            _sourceMetadataService = sourceMetadataService;
            _targetSchemaService = targetSchemaService;
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

                var result = new SyncMetadataResult();
                var sourceTables = context.SourceTables ?? new List<Table>();
                var sourceTableMap = sourceTables.ToDictionary(table => table.Name, StringComparer.OrdinalIgnoreCase);

                foreach (var synchronizedTableName in context.SynchronizedTableNames)
                {
                    if (!sourceTableMap.TryGetValue(synchronizedTableName, out var sourceTable))
                    {
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false,
                            "Table is missing in source metadata."));
                        continue;
                    }

                    if (!context.TargetTableExists.TryGetValue(synchronizedTableName, out var targetTableExists))
                    {
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false,
                            "Target table existence could not be determined."));
                        continue;
                    }

                    try
                    {
                        if (!targetTableExists)
                        {
                            _targetSchemaService.CreateTargetTable(sourceTable);
                            result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, true));
                            continue;
                        }

                        var isUpToDate = _targetSchemaService.IsTargetTableSchemaUpToDate(sourceTable);
                        if (!isUpToDate)
                        {
                            _targetSchemaService.UpdateTargetTable(sourceTable);
                        }

                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, true));
                    }
                    catch (Exception ex)
                    {
                        LogIfEnabled(LogLevel.Error, ex,
                            "Synchronization failed for table {TableName}.", synchronizedTableName);
                        result.TablesSyncResult.Add(new TableSyncResult(synchronizedTableName, false, ex.Message));
                    }
                }

                LogIfEnabled(LogLevel.Information, "Metadata synchronization completed for {Count} table(s).",
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
