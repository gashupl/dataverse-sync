using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Target;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    //See ADR-0001: docs/adr/0001-excluding-database-target-repositories-from-code-coverage.md
    [ExcludeFromCodeCoverage]
    public class TargetDataRepository : LoggingServiceBase<TargetDataRepository>, ITargetDataRepository
    {
        private readonly string _connectionString;

        public TargetDataRepository(string connectionString, ILogger<TargetDataRepository> logger)
            : base(logger)
        {
            _connectionString = connectionString;
        }

        public TargetRecordModificationResult InsertRecord(TargetRecord record)
        {
            LogIfEnabled(LogLevel.Information, "Inserting record into table '{TableName}'...", record.TableName);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    LogIfEnabled(LogLevel.Information, "Connection to target database established successfully.");

                    var escapedTableName = EscapeSqlIdentifier(record.TableName);
                    var columnNamesList = new StringBuilder();
                    var parameterNamesList = new StringBuilder();

                    for (int i = 0; i < record.Columns.Count; i++)
                    {
                        if (i > 0)
                        {
                            columnNamesList.Append(", ");
                            parameterNamesList.Append(", ");
                        }

                        columnNamesList.Append(EscapeSqlIdentifier(record.Columns[i].ColumnName));
                        parameterNamesList.Append($"@Param{i}");
                    }

                    var query = $"INSERT INTO {escapedTableName} ({columnNamesList}) VALUES ({parameterNamesList})";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        for (int i = 0; i < record.Columns.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@Param{i}", record.Columns[i].Value ?? DBNull.Value);
                        }

                        LogIfEnabled(LogLevel.Information, "Executing query to insert record: {Query}", query);
                        command.ExecuteNonQuery();
                        LogIfEnabled(LogLevel.Information, "Record inserted successfully into table '{TableName}'.", record.TableName);

                        return new TargetRecordModificationResult { Success = true };
                    }
                }
                catch (Exception ex)
                {
                    LogIfEnabled(LogLevel.Error, "An error occurred while inserting record into table '{TableName}': {ErrorMessage}", record.TableName, ex.Message);
                    return new TargetRecordModificationResult { Success = false, Message = ex.Message };
                }
            }
        }

        public TargetRecordModificationResult UpdateRecord(TargetRecord record)
        {
            LogIfEnabled(LogLevel.Information, "Updating record in table '{TableName}'...", record.TableName);

            var primaryKeyColumn = record.Columns.FirstOrDefault(c => c.IsPrimaryKey);
            if (primaryKeyColumn == null)
            {
                var message = $"No primary key column found for table '{record.TableName}'. Update operation requires a primary key.";
                LogIfEnabled(LogLevel.Error, message);
                return new TargetRecordModificationResult { Success = false, Message = message };
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    LogIfEnabled(LogLevel.Information, "Connection to target database established successfully.");

                    var nonPrimaryKeyColumns = record.Columns.Where(c => !c.IsPrimaryKey).ToList();

                    if (nonPrimaryKeyColumns.Count == 0)
                    {
                        var message = $"No columns to update in table '{record.TableName}'. Only primary key column exists.";
                        LogIfEnabled(LogLevel.Warning, message);
                        return new TargetRecordModificationResult { Success = true, Message = message };
                    }

                    var escapedTableName = EscapeSqlIdentifier(record.TableName);
                    var escapedPrimaryKeyColumnName = EscapeSqlIdentifier(primaryKeyColumn.ColumnName);
                    var setClauseBuilder = new StringBuilder();

                    for (int i = 0; i < nonPrimaryKeyColumns.Count; i++)
                    {
                        if (i > 0)
                        {
                            setClauseBuilder.Append(", ");
                        }

                        var escapedColumnName = EscapeSqlIdentifier(nonPrimaryKeyColumns[i].ColumnName);
                        setClauseBuilder.Append($"{escapedColumnName} = @UpdateParam{i}");
                    }

                    var query = $"UPDATE {escapedTableName} SET {setClauseBuilder} WHERE {escapedPrimaryKeyColumnName} = @PrimaryKeyParam";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        for (int i = 0; i < nonPrimaryKeyColumns.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@UpdateParam{i}", nonPrimaryKeyColumns[i].Value ?? DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@PrimaryKeyParam", primaryKeyColumn.Value ?? DBNull.Value);

                        LogIfEnabled(LogLevel.Information, "Executing query to update record: {Query}", query);
                        command.ExecuteNonQuery();
                        LogIfEnabled(LogLevel.Information, "Record updated successfully in table '{TableName}'.", record.TableName);
                        return new TargetRecordModificationResult { Success = true };
                    }
                }
                catch (Exception ex)
                {
                    LogIfEnabled(LogLevel.Error, "An error occurred while updating record in table '{TableName}': {ErrorMessage}", record.TableName, ex.Message);
                    return new TargetRecordModificationResult { Success = false, Message = ex.Message };
                }
            }
        }

        public TargetRecordModificationResult DeleteRecord(TargetRecord record)
        {
            LogIfEnabled(LogLevel.Information, "Deleting record from table '{TableName}'...", record.TableName);

            var primaryKeyColumn = record.Columns.FirstOrDefault(c => c.IsPrimaryKey);
            if (primaryKeyColumn == null)
            {
                var message = $"No primary key column found for table '{record.TableName}'. Delete operation requires a primary key.";
                LogIfEnabled(LogLevel.Error, message);
                return new TargetRecordModificationResult { Success = false, Message = message };
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    LogIfEnabled(LogLevel.Information, "Connection to target database established successfully.");

                    var escapedTableName = EscapeSqlIdentifier(record.TableName);
                    var escapedPrimaryKeyColumnName = EscapeSqlIdentifier(primaryKeyColumn.ColumnName);
                    var query = $"DELETE FROM {escapedTableName} WHERE {escapedPrimaryKeyColumnName} = @PrimaryKeyParam";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PrimaryKeyParam", primaryKeyColumn.Value ?? DBNull.Value);

                        LogIfEnabled(LogLevel.Information, "Executing query to delete record: {Query}", query);
                        command.ExecuteNonQuery();
                        LogIfEnabled(LogLevel.Information, "Record deleted successfully from table '{TableName}'.", record.TableName);

                        return new TargetRecordModificationResult { Success = true };
                    }
                }
                catch (Exception ex)
                {
                    LogIfEnabled(LogLevel.Error, "An error occurred while deleting record from table '{TableName}': {ErrorMessage}", record.TableName, ex.Message);
                    return new TargetRecordModificationResult { Success = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// Escapes a SQL identifier (table name or column name) for safe use in SQL queries.
        /// </summary>
        private static string EscapeSqlIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));
            }

            if (!IsValidSqlIdentifier(identifier))
            {
                throw new ArgumentException($"Invalid SQL identifier: '{identifier}'. Identifiers must contain only alphanumeric characters, underscores, and start with a letter or underscore.", nameof(identifier));
            }

            return $"[{identifier}]";
        }

        /// <summary>
        /// Validates that a SQL identifier contains only safe characters.
        /// </summary>
        private static bool IsValidSqlIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier) || identifier.Length > 128)
            {
                return false;
            }

            if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            {
                return false;
            }

            return identifier.All(c => char.IsLetterOrDigit(c) || c == '_');
        }
    }
}
