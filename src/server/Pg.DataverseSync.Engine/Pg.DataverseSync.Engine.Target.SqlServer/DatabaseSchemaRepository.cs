using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Model;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    //See ADR-0001: docs/adr/0001-excluding-database-schema-repository-from-code-coverage.md
    [ExcludeFromCodeCoverage]
    public class DatabaseSchemaRepository :  LoggingServiceBase<DatabaseSchemaRepository>, ITargetSchemaRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseSchemaRepository> _logger;

        public DatabaseSchemaRepository(string connectionString, ILogger<DatabaseSchemaRepository> logger) 
            : base(logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public TargetSchemaModificationResult CreateTable(Table sourceTable)
        {
            LogIfEnabled(LogLevel.Information, "Creating table '{TableName}' in target database...", sourceTable.Name); 

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    _logger.LogInformation("Connection to target database established successfully.");

                    var query = CreateTableQueryGenerator.Generate(sourceTable);
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        LogIfEnabled(LogLevel.Information, "Executing query to create table: {Query}", query);
                        command.ExecuteNonQuery();
                        _logger.LogInformation("Table created successfully.");
                        return new TargetSchemaModificationResult { Success = SchemaModificationResult.Success };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);
                    return new TargetSchemaModificationResult { Success = SchemaModificationResult.Failure, Message = ex.Message };
                }
            }
        }


        public bool TableExists(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                return (int)command.ExecuteScalar() > 0;
            }
        }

        public TargetSchemaModificationResult UpdateTable(Table sourceTable)
        {
            SqlTable targetTable;
            try
            {
                targetTable = GetTable(sourceTable.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve target table '{SourceTableName}': {Message}", sourceTable.Name, ex.Message);
                return new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResult.Failure,
                    Message = $"Failed to retrieve target table '{sourceTable.Name}': {ex.Message}"
                };
            }

            //TODO: Find out why there are always following columns on the list of ones to be added or removed (alternately):
            //entityimage - "VARBINARY(MAX)"
            //exchangerate - DECIMAL(38, 0)
            var columnsToRemove = TableSchemaComparer.GetColumnsToBeRemoved(sourceTable, targetTable);
            var columnsToAdd = TableSchemaComparer.GetColumnsToBeAdded(sourceTable, targetTable);
            var modifiedColumns = TableSchemaComparer.GetModifiedColumns(sourceTable, targetTable);

            columnsToRemove = TableSchemaComparer.MergeColumns(columnsToRemove, modifiedColumns.TargetChanges);
            columnsToAdd = TableSchemaComparer.MergeColumns(columnsToAdd, modifiedColumns.SourceChanges);

            var errors = new List<string>();
            var totalOperations = columnsToRemove.Count + columnsToAdd.Count;
            var failedOperations = 0;

            foreach (var columnName in columnsToRemove.Select(c => c.Name).OfType<string>())
            {
                try
                {
                    RemoveTargetColumn(targetTable.Name, columnName);
                }
                catch (Exception ex)
                {
                    failedOperations++;
                    var message = $"Failed to remove column '{columnName}' from table '{targetTable.Name}': {ex.Message}";
                    _logger.LogError(ex, "Failed to remove column '{ColumnName}' from table '{TableName}': {ErrorMessage}", columnName, targetTable.Name, ex.Message);
                    errors.Add(message);
                }
            }

            foreach (var column in columnsToAdd)
            {
                try
                {
                    AddTargetColumn(targetTable.Name, column);
                }
                catch (Exception ex)
                {
                    failedOperations++;
                    var message = $"Failed to add column '{column.Name}' to table '{targetTable.Name}': {ex.Message}";
                    _logger.LogError(ex, "Failed to add column '{ColumnName}' to table '{TableName}': {ErrorMessage}", column.Name, targetTable.Name, ex.Message);
                    errors.Add(message);
                }
            }

            if (failedOperations == 0)
            {
                LogIfEnabled(LogLevel.Information, "Table '{TargetTableName}' updated successfully.", targetTable.Name);
                return new TargetSchemaModificationResult { Success = SchemaModificationResult.Success };
            }

            if (failedOperations == totalOperations)
            {
                return new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResult.Failure,
                    Message = string.Join(Environment.NewLine, errors)
                };
            }

            return new TargetSchemaModificationResult
            {
                Success = SchemaModificationResult.PartialSuccess,
                Message = string.Join(Environment.NewLine, errors)
            };
        }

        internal SqlTable GetTable(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Retrieve table columns
                var command = new SqlCommand(
                    @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity
                      FROM INFORMATION_SCHEMA.COLUMNS
                      WHERE TABLE_NAME = @TableName", connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                var columns = new List<Column>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["COLUMN_NAME"]?.ToString() ?? string.Empty; 
                        string dataType = reader["DATA_TYPE"]?.ToString() ?? string.Empty;
                        if (StringComparer.OrdinalIgnoreCase.Equals(dataType, "nvarchar"))
                        {
                            dataType = "NVARCHAR(MAX)";
                        }
                        bool isNullable = reader["IS_NULLABLE"]?.ToString() == "YES";
                        bool isIdentity = reader["IsIdentity"] != null && (int)reader["IsIdentity"] == 1;

                        columns.Add(new Column(name, dataType, isNullable: isNullable, isIdentity: isIdentity));
                    }
                }

                return new SqlTable(tableName, columns);
            }
        }

        private void AddTargetColumn(string tableName, Column column)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Build the ALTER TABLE statement with parameters
                var sqlBuilder = new StringBuilder("ALTER TABLE ");
                sqlBuilder.Append($"[{tableName}] ADD [{column.Name}] {column.DataType}");

                if (!column.IsNullable)
                    sqlBuilder.Append(" NOT NULL");
                else
                    sqlBuilder.Append(" NULL");

                if (column.IsIdentity)
                    sqlBuilder.Append(" IDENTITY(1,1)");

                if (column.IsPrimaryKey)
                    sqlBuilder.Append(" PRIMARY KEY");

                using (var command = new SqlCommand(sqlBuilder.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                    LogIfEnabled(
                        LogLevel.Information,
                        "Column {ColumnName} added successfully to table {TableName}.",
                        column.Name ?? string.Empty,
                        tableName);
                }
            }
        }

        private void RemoveTargetColumn(string tableName, string columnName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sqlBuilder = new StringBuilder("ALTER TABLE ");
                sqlBuilder.Append($"[{tableName}] DROP COLUMN [{columnName}]");

                using (var command = new SqlCommand(sqlBuilder.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
