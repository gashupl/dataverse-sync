using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
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

            return ExecuteRecordModification(
                record,
                connection => CreateInsertCommand(connection, record),
                "Executing query to insert record: {Query}",
                "Record inserted successfully into table '{TableName}'.",
                "An error occurred while inserting record into table '{TableName}': {ErrorMessage}");
        }

        public TargetRecordModificationResult UpdateRecord(TargetRecord record)
        {
            LogIfEnabled(LogLevel.Information, "Updating record in table '{TableName}'...", record.TableName);

            var validationResult = ValidatePrimaryKey(record, "Update", out TargetColumn? primaryKeyColumn);
            if (validationResult != null)
            {
                return validationResult;
            }

            var nonPrimaryKeyColumns = record.Columns.Where(c => !c.IsPrimaryKey).ToList();
            if (nonPrimaryKeyColumns.Count == 0)
            {
                var message = $"No columns to update in table '{record.TableName}'. Only primary key column exists.";
                LogIfEnabled(LogLevel.Warning, message);
                return new TargetRecordModificationResult { Success = true, Message = message };
            }

            return ExecuteRecordModification(
                record,
                connection => CreateUpdateCommand(connection, record, primaryKeyColumn!, nonPrimaryKeyColumns),
                "Executing query to update record: {Query}",
                "Record updated successfully in table '{TableName}'.",
                "An error occurred while updating record in table '{TableName}': {ErrorMessage}");
        }

        public TargetRecordModificationResult DeleteRecord(TargetRecord record)
        {
            LogIfEnabled(LogLevel.Information, "Deleting record from table '{TableName}'...", record.TableName);

            var validationResult = ValidatePrimaryKey(record, "Delete", out TargetColumn? primaryKeyColumn);
            if (validationResult != null)
            {
                return validationResult;
            }

            return ExecuteRecordModification(
                record,
                connection => CreateDeleteCommand(connection, record, primaryKeyColumn!),
                "Executing query to delete record: {Query}",
                "Record deleted successfully from table '{TableName}'.",
                "An error occurred while deleting record from table '{TableName}': {ErrorMessage}");
        }

        private TargetRecordModificationResult ExecuteRecordModification(
            TargetRecord record,
            Func<SqlConnection, SqlCommand> commandFactory,
            string queryLogMessage,
            string successLogMessage,
            string errorLogMessage)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    LogIfEnabled(LogLevel.Information, "Connection to target database established successfully.");

                    using (SqlCommand command = commandFactory(connection))
                    {
                        LogIfEnabled(LogLevel.Information, queryLogMessage, command.CommandText);
                        command.ExecuteNonQuery();
                        LogIfEnabled(LogLevel.Information, successLogMessage, record.TableName);

                        return new TargetRecordModificationResult { Success = true };
                    }
                }
                catch (Exception ex)
                {
                    LogIfEnabled(LogLevel.Error, errorLogMessage, record.TableName, ex.Message);
                    return new TargetRecordModificationResult { Success = false, Message = ex.Message };
                }
            }
        }

        private static SqlCommand CreateInsertCommand(SqlConnection connection, TargetRecord record)
        {
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
                parameterNamesList.Append($"@Param{i}"); // NOSONAR - Parameter name placeholder
            }

            var query = $"INSERT INTO {escapedTableName} ({columnNamesList}) VALUES ({parameterNamesList})"; // NOSONAR - Identifiers are escaped, values are parameterized
            SqlCommand command = new SqlCommand(query, connection);
            AddParameters(command, record.Columns, "@Param");

            return command;
        }

        private static SqlCommand CreateUpdateCommand(
            SqlConnection connection,
            TargetRecord record,
            TargetColumn primaryKeyColumn,
            IReadOnlyList<TargetColumn> nonPrimaryKeyColumns)
        {
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
                setClauseBuilder.Append($"{escapedColumnName} = @UpdateParam{i}"); // NOSONAR - Identifiers are escaped, values are parameterized
            }

            var query = $"UPDATE {escapedTableName} SET {setClauseBuilder} WHERE {escapedPrimaryKeyColumnName} = @PrimaryKeyParam"; // NOSONAR - Identifiers are escaped, values are parameterized
            SqlCommand command = new SqlCommand(query, connection);
            AddParameters(command, nonPrimaryKeyColumns, "@UpdateParam");
            command.Parameters.AddWithValue("@PrimaryKeyParam", primaryKeyColumn.Value ?? DBNull.Value);

            return command;
        }

        private static SqlCommand CreateDeleteCommand(SqlConnection connection, TargetRecord record, TargetColumn primaryKeyColumn)
        {
            var escapedTableName = EscapeSqlIdentifier(record.TableName);
            var escapedPrimaryKeyColumnName = EscapeSqlIdentifier(primaryKeyColumn.ColumnName);
            var query = $"DELETE FROM {escapedTableName} WHERE {escapedPrimaryKeyColumnName} = @PrimaryKeyParam"; // NOSONAR - Identifiers are escaped, values are parameterized
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PrimaryKeyParam", primaryKeyColumn.Value ?? DBNull.Value);

            return command;
        }

        private TargetRecordModificationResult? ValidatePrimaryKey(
            TargetRecord record,
            string operationName,
            out TargetColumn? primaryKeyColumn)
        {
            primaryKeyColumn = record.Columns.FirstOrDefault(c => c.IsPrimaryKey);
            if (primaryKeyColumn != null)
            {
                return null;
            }

            var message = $"No primary key column found for table '{record.TableName}'. {operationName} operation requires a primary key.";
            LogIfEnabled(LogLevel.Error, message);
            return new TargetRecordModificationResult { Success = false, Message = message };
        }

        private static void AddParameters(SqlCommand command, IReadOnlyList<TargetColumn> columns, string parameterPrefix)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                command.Parameters.AddWithValue($"{parameterPrefix}{i}", columns[i].Value ?? DBNull.Value); // NOSONAR - Parameter name placeholder
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

            return $"[{identifier}]"; // NOSONAR - Safe identifier escaping
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
