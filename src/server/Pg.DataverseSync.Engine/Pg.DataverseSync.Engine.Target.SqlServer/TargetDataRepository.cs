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

                    var columnNames = string.Join(", ", record.Columns.Select(c => $"[{c.ColumnName}]"));
                    var parameterNames = string.Join(", ", record.Columns.Select((c, i) => $"@Param{i}"));
                    var query = $"INSERT INTO [{record.TableName}] ({columnNames}) VALUES ({parameterNames})";

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

                    var setClause = string.Join(", ", record.Columns
                        .Where(c => !c.IsPrimaryKey)
                        .Select((c, i) => $"[{c.ColumnName}] = @UpdateParam{i}"));

                    if (string.IsNullOrEmpty(setClause))
                    {
                        var message = $"No columns to update in table '{record.TableName}'. Only primary key column exists.";
                        LogIfEnabled(LogLevel.Warning, message);
                        return new TargetRecordModificationResult { Success = true, Message = message };
                    }

                    var query = $"UPDATE [{record.TableName}] SET {setClause} WHERE [{primaryKeyColumn.ColumnName}] = @PrimaryKeyParam";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        int paramIndex = 0;
                        foreach (var column in record.Columns.Where(c => !c.IsPrimaryKey))
                        {
                            command.Parameters.AddWithValue($"@UpdateParam{paramIndex}", column.Value ?? DBNull.Value);
                            paramIndex++;
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

                    var query = $"DELETE FROM [{record.TableName}] WHERE [{primaryKeyColumn.ColumnName}] = @PrimaryKeyParam";

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
    }
}
