using Pg.DataverseSync.Engine.Source.Model;
namespace Pg.DataverseSync.Engine.Source
{
    public interface IMetadataReader
    {
        List<Table>? GetTables();

        List<Column> GetColumns(string tableName);

    }
}
