using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface ITargetSchemaRepository
    {
        bool TableExists(string tableName);

        TargetSchemaModificationResult CreateTable(Table table); 
    }
}
