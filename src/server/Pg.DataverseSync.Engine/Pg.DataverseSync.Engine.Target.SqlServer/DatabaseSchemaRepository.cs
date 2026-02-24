using Microsoft.Data.SqlClient;
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

    }
}
