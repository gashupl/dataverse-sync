using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface IMetadataRepository
    {
        List<Table>? GetTables();

        List<Column> GetColumns(string tableName);

    }
}
