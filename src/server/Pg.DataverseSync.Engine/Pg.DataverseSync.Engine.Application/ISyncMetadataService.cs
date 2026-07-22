using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ISyncMetadataService
    {
        List<Table>? GetTables();
    }
}
