using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Domain
{
    public interface ISourceMetadataService
    {
        List<Table> GetTables();
    }
}
