using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target
{
    public interface IDataStructureRepository
    {
        UpsertTableResult UpsertTable(Table table); 
    }
}
