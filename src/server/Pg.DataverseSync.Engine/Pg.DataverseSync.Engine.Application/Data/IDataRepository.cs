using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface IDataRepository
    {
        List<Entity> GetActiveSyncTables();
        List<Entity> GetRecords(string tableName, List<string> columns, FilterExpression? filter);

        List<pg_synchronizationdetails> GetSyncDetails(string tableName, pg_TargetRepositorySet targetRepositorySet);
    }
}
