using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    //See ADR-0001: docs/adr/0001-excluding-database-schema-repository-from-code-coverage.md
    [ExcludeFromCodeCoverage]
    public class DatabaseSchemaRepository : ITargetSchemaRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseSchemaRepository> _logger;

        public DatabaseSchemaRepository(string connectionString, ILogger<DatabaseSchemaRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public TargetSchemaModificationResult CreateTable(Table sourceTable)
        {
            _logger.LogInformation($"Creating table '{sourceTable.Name}' in target database...");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    _logger.LogInformation("Connection to target database established successfully.");

                    var query = CreateTableQueryGenerator.Generate(sourceTable); 
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        _logger.LogInformation($"Executing query to create table: {query}");
                        command.ExecuteNonQuery();
                        _logger.LogInformation("Table created successfully.");
                        return new TargetSchemaModificationResult { Success = SchemaModificationResultEnum.Success };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                    return new TargetSchemaModificationResult { Success = SchemaModificationResultEnum.Failure, Message = ex.Message };
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
                _logger.LogError($"Failed to retrieve target table '{sourceTable.Name}': {ex.Message}");
                return new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResultEnum.Failure,
                    Message = $"Failed to retrieve target table '{sourceTable.Name}': {ex.Message}"
                };
            }

            var tableComparer = new TableSchemaComparer(_logger);

            var columnsToRemove = tableComparer.GetColumnsToBeRemoved(sourceTable, targetTable);
            var columnsToAdd = tableComparer.GetColumnsToBeAdded(sourceTable, targetTable);
            var modifiedColumns = tableComparer.GetModifiedColumns(sourceTable, targetTable);

            columnsToRemove = TableSchemaComparer.MergeColumns(columnsToRemove, modifiedColumns.TargetChanges);
            columnsToAdd = TableSchemaComparer.MergeColumns(columnsToAdd, modifiedColumns.SourceChanges);

            var errors = new List<string>();
            var totalOperations = columnsToRemove.Count + columnsToAdd.Count;
            var failedOperations = 0;

            foreach (var column in columnsToRemove)
            {
                try
                {
                    RemoveTargetColumn(targetTable.Name, column.Name);
                }
                catch (Exception ex)
                {
                    failedOperations++;
                    var message = $"Failed to remove column '{column.Name}' from table '{targetTable.Name}': {ex.Message}";
                    _logger.LogError(message);
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
                    _logger.LogError(message);
                    errors.Add(message);
                }
            }

            if (failedOperations == 0)
            {
                _logger.LogInformation($"Table '{targetTable.Name}' updated successfully.");
                return new TargetSchemaModificationResult { Success = SchemaModificationResultEnum.Success };
            }

            if (failedOperations == totalOperations)
            {
                return new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResultEnum.Failure,
                    Message = string.Join(Environment.NewLine, errors)
                };
            }

            return new TargetSchemaModificationResult
            {
                Success = SchemaModificationResultEnum.PartialSuccess,
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
                        string name = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        if (dataType != null && dataType.ToLower().Equals("nvarchar"))
                        {
                            dataType = "NVARCHAR(MAX)";
                        }
                        bool isNullable = reader["IS_NULLABLE"].ToString() == "YES";
                        bool isIdentity = (int)reader["IsIdentity"] == 1;

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
                var command = new SqlCommand(
                    $"ALTER TABLE {tableName} ADD {column.Name} {column.DataType}" +
                    $"{(column.IsNullable ? " NULL" : " NOT NULL")}" +
                    $"{(column.IsIdentity ? " IDENTITY(1,1)" : "")}" +
                    $"{(column.IsPrimaryKey ? " PRIMARY KEY" : "")}", connection);
                command.ExecuteNonQuery();
                _logger.LogInformation($"Column {column.Name} added successfully to table {tableName}.");
            }
        }

        private void RemoveTargetColumn(string tableName, string columnName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    $"ALTER TABLE {tableName} DROP COLUMN {columnName}", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
