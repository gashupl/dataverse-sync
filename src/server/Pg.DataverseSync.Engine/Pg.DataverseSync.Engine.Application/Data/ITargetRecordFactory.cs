using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface ITargetRecordFactory
    {
        TargetRecord CreateForInsert(Entity entity);
        TargetRecord CreateForUpdate(Entity entity);
        TargetRecord CreateForDelete(EntityReference entityReference);
    }
}
