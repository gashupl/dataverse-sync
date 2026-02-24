using Microsoft.Data.SqlClient;
using Pg.DataverseSync.Engine.Core.Model;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    //See ADR-0001: docs/adr/0001-excluding-database-schema-repository-from-code-coverage.md
    [ExcludeFromCodeCoverage]
    public class DatabaseSchemaRepository : IDatabaseSchemaRepository
    {
        private readonly string _connectionString;

        public DatabaseSchemaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SchemaModificationResult CreateTargetTable(Table table)
        {
            //TODO: Implement error handling and return appropriate SchemaModificationResult based on the outcome of the operation.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(CreateTableQueryGenerator.Generate(table), connection))
                    {
                        command.ExecuteNonQuery();
                       // _logger.LogInformation("Table created successfully.");
                       return new SchemaModificationResult { Success = SchemaModificationResultEnum.Success };
                    }
                }
                catch (Exception ex)
                {
                    // _logger.LogError($"An error occurred: {ex.Message}");
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
