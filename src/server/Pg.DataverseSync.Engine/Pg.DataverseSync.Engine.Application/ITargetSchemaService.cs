using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ITargetSchemaService
    {
        bool TargetTableExists(string tableName);

        void CreateTargetTable(Table table);

        bool IsTargetTableSchemaUpToDate(Table table);

        void UpdateTargetTable(Table table);
    }
}
