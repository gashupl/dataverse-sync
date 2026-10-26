using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Application
{
    public class SourceMetadataService : LoggingServiceBase<SourceMetadataService>, ISourceMetadataService
    {
        private readonly IMetadataRepository _metadataRepo;
        private readonly IDataRepository _dataRepository; 

        public SourceMetadataService(IMetadataRepository metadataRepo, 
            IDataRepository dataRepository, ILogger<SourceMetadataService> logger)
            : base(logger)
        {
            _metadataRepo = metadataRepo;   
            _dataRepository = dataRepository;
        }

        public List<string> GetSynchronizedTableNames()
        {
            
            var names = _dataRepository
                .GetActiveSyncTables().Select(t => t.Attributes[pg_synctable.Fields.pg_name] as string)
                .Where(n => n != null);

            return names.ToList()!; 
        }

        public List<Table>? GetTables(List<string> tableNames)
        {
            LogIfEnabled(LogLevel.Information, "Getting tables from source metadata service...");
            try
            {
                var allTables = _metadataRepo.GetTables();
                if(allTables == null || allTables.Count == 0)
                {
                    LogIfEnabled(LogLevel.Warning, "No tables found in source metadata service.");
                    return null; 
                }
                else
                {
                    var tables = allTables.Where(t => tableNames.Contains(t.Name)); 
                    foreach (var table in tables)
                    {
                        table.Columns = _metadataRepo.GetColumns(table.Name);
                        LogIfEnabled(LogLevel.Information, "Successfully retrieved {ColumnCount} columns for table {TableName}.", table.Columns.Count, table.Name);
                    }
                    LogIfEnabled(LogLevel.Information, "Successfully retrieved {TableCount} table.", tables.Count());
                    return tables.ToList();
                }
                

            }
            catch(ReadMetadataException ex)
            {
                var message = "An error occurred while reading metadata for tables.";
                LogIfEnabled(LogLevel.Error, ex, message);
                throw new ApplicationServiceException(message, ex);
            }
        }


        public SyncDetailsModificationResult UpsertSyncDetails(string tableName, string checksum, pg_TargetRepositorySet targetRepositorySet)
        {
            try
            {
                var existingDetails = _dataRepository.GetSyncDetails(tableName, targetRepositorySet).FirstOrDefault();

                if (existingDetails != null)
                {
                    _dataRepository.UpdateRecord(new pg_synchronizationdetails(existingDetails.Id)
                    {
                        pg_checksum = checksum,
                        pg_lastsynchronizationdate = DateTime.UtcNow
                    });

                    return new SyncDetailsModificationResult { Success = ModificationResult.Success };
                }

                var syncTable = _dataRepository.GetSyncTable(tableName);

                if (syncTable == null)
                {
                    var notFoundMessage = $"Sync table '{tableName}' was not found.";
                    LogIfEnabled(LogLevel.Warning, notFoundMessage);

                    return new SyncDetailsModificationResult
                    {
                        Success = ModificationResult.Failure,
                        Message = notFoundMessage
                    };
                }

                _dataRepository.CreateRecord(new pg_synchronizationdetails
                {
                    pg_checksum = checksum,
                    pg_lastsynchronizationdate = DateTime.UtcNow,
                    pg_targetrepository = targetRepositorySet,
                    pg_synctableId = new EntityReference(pg_synctable.EntityLogicalName, syncTable.Id)
                });

                return new SyncDetailsModificationResult { Success = ModificationResult.Success };
            }
            catch (Exception ex)
            {
                var message = "An error occurred while upserting sync details for table {TableName}.";
                LogIfEnabled(LogLevel.Error, ex, message, tableName);

                return new SyncDetailsModificationResult
                {
                    Success = ModificationResult.Failure,
                    Message = ex.Message
                };
            }
        }
    }
}
