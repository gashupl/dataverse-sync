using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ITargetSchemaService
    {
        bool TargetTableExists(string tableName);

        TargetSchemaModificationResult UpsertTargetTable(Table table);

    }
}
