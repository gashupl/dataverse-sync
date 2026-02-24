using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Domain.Target
{
    public interface ITargetDataStructureService
    {
        UpsertTableResult UpsertTable(Table table); 
    }
}
