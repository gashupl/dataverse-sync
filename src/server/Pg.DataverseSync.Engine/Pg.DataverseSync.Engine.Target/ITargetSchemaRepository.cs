using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target
{
    public interface ITargetSchemaRepository
    {
        bool TableExists(string tableName);

        TargetSchemaModificationResult CreateTable(Table table);

        TargetSchemaModificationResult UpdateTable(Table table);
    }
}
