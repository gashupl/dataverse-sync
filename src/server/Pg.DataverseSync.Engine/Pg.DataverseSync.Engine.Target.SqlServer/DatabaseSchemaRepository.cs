using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    //See ADR-0001: docs/adr/0001-excluding-database-schema-repository-from-code-coverage.md
    [ExcludeFromCodeCoverage]
    public class DatabaseSchemaRepository : IDatabaseSchemaRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseSchemaRepository> _logger;

        public DatabaseSchemaRepository(string connectionString, ILogger<DatabaseSchemaRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public SchemaModificationResult CreateTargetTable(Table table)
        {
            _logger.LogInformation($"Creating table '{table.Name}' in target database...");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    _logger.LogInformation("Connection to target database established successfully.");

                    var query = CreateTableQueryGenerator.Generate(table); 
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        _logger.LogInformation($"Executing query to create table: {query}");
                        command.ExecuteNonQuery();
                        _logger.LogInformation("Table created successfully.");
                        return new SchemaModificationResult { Success = SchemaModificationResultEnum.Success };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                    return new SchemaModificationResult { Success = SchemaModificationResultEnum.Failure, Message = ex.Message };
                }
            }
        }

        public bool TargetTableExists(string tableName)
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

        public SchemaModificationResult UpdateTargetTable(Table table)
        {
            throw new NotImplementedException();
        }
    }
}
