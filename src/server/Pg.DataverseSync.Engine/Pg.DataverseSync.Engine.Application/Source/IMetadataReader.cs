using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Source
{
    public interface IMetadataReader
    {
        List<Table>? GetTables();

        List<Column> GetColumns(string tableName);

    }
}
