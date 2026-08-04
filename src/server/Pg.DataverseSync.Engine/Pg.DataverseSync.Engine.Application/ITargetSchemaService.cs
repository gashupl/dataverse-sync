using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ITargetSchemaService
    {
        bool TargetTableExists(string tableName);

        TargetSchemaModificationResult UpsertTargetTable(Table table);

    }
}
