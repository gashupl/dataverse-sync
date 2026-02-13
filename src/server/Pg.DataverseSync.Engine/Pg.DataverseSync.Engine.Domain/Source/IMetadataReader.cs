using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Domain.Source
{
    public interface IMetadataReader
    {
        List<Table>? GetTables();

        List<Column> GetColumns(string tableName);

    }
}
