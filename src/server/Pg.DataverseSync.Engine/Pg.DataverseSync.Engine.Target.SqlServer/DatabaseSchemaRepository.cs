using Microsoft.Data.SqlClient;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
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
