using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public interface IDatabaseSchemaRepository
    {
        bool TargetTableExists(string tableName);

        SchemaModificationResult CreateTargetTable(Table table);

        SchemaModificationResult UpdateTargetTable(Table table);

    }

}