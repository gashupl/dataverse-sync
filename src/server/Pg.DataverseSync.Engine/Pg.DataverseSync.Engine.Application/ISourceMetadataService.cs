using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ISourceMetadataService
    {
        List<string> GetSynchronizedTableNames();

        List<Table>? GetTables();
    }
}
