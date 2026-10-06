using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Target;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pg.DataverseSync.Engine.Application
{
    public class DataLoadService : LoggingServiceBase<DataLoadService>, IDataLoadService
    {
        private readonly IDataRepository _dataRepository;
        private readonly ISourceMetadataService _sourceMetadataService;
        private readonly ITargetDataRepository _targetDataRepository;
        private readonly ITargetRecordFactory _targetRecordFactory;

        public DataLoadService(
            IDataRepository dataRepository,
            ISourceMetadataService sourceMetadataService,
            ITargetDataRepository targetDataRepository,
            ITargetRecordFactory targetRecordFactory,
            ILogger<DataLoadService> logger)
            : base(logger)
        {
            _dataRepository = dataRepository;
            _sourceMetadataService = sourceMetadataService;
            _targetDataRepository = targetDataRepository;
            _targetRecordFactory = targetRecordFactory;
        }

        public List<TargetRecordModificationResult> LoadInitialData(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            LogIfEnabled(LogLevel.Information, "Starting initial data load for table '{TableName}'.", tableName);

            var sourceTable = GetSourceTable(tableName);
            if (sourceTable == null)
            {
                LogIfEnabled(LogLevel.Warning, "Source table '{TableName}' was not found. Initial data load skipped.", tableName);
                return new List<TargetRecordModificationResult>();
            }

            var columns = GetColumnNames(sourceTable);
            if (columns.Count == 0)
            {
                LogIfEnabled(LogLevel.Warning, "Source table '{TableName}' does not contain any columns. Initial data load skipped.", tableName);
                return new List<TargetRecordModificationResult>();
            }

            var records = _dataRepository.GetRecords(tableName, columns, null);
            LogIfEnabled(LogLevel.Information,
                "Retrieved {RecordCount} records from source table '{TableName}'.",
                records.Count,
                tableName);

            var results = LoadRecords(tableName, records);

            LogIfEnabled(LogLevel.Information,
                "Initial data load finished for table '{TableName}'. Processed {RecordCount} records.",
                tableName,
                results.Count);

            return results;
        }

        private Core.Model.Table? GetSourceTable(string tableName)
        {
            return _sourceMetadataService.GetTables(new List<string> { tableName })?
                .FirstOrDefault(table => table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
        }

        private static List<string> GetColumnNames(Core.Model.Table sourceTable)
        {
            return sourceTable.Columns
                .Select(column => column.Name)
                .Where(columnName => !string.IsNullOrWhiteSpace(columnName))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }


        private List<TargetRecordModificationResult> LoadRecords(string tableName, List<Microsoft.Xrm.Sdk.Entity> records)
        {
            var results = new List<TargetRecordModificationResult>();

            foreach (var record in records)
            {
                results.Add(InsertRecord(tableName, record));
            }

            return results;
        }

        private TargetRecordModificationResult InsertRecord(string tableName, Microsoft.Xrm.Sdk.Entity record)
        {
            try
            {
                var targetRecord = _targetRecordFactory.CreateForInsert(record);
                var result = _targetDataRepository.InsertRecord(targetRecord);

                LogInsertResult(tableName, result);

                return result;
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error,
                    ex,
                    "An exception occurred while inserting initial data into table '{TableName}': {ErrorMessage}",
                    tableName,
                    ex.Message);

                return new TargetRecordModificationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private void LogInsertResult(string tableName, TargetRecordModificationResult result)
        {
            if (result.Success)
            {
                LogIfEnabled(LogLevel.Information,
                    "Initial data load insert succeeded for table '{TableName}'.",
                    tableName);
            }
            else
            {
                LogIfEnabled(LogLevel.Warning,
                    "Initial data load insert failed for table '{TableName}': {Message}",
                    tableName,
                    result.Message!);
            }
        }
    }
}
