using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface IDataRepository
    {
        List<Entity> GetActiveSyncTables();

        pg_synctable? GetSyncTable(string name); 

        List<pg_synchronizationdetails> GetSyncDetails(string tableName, pg_TargetRepositorySet targetRepositorySet);
        
        List<Entity> GetRecords(string tableName, List<string> columns, FilterExpression? filter);

        public Guid CreateRecord(Entity record);

        public void UpdateRecord(Entity record); 

        public void DeleteRecord(Guid id, string tableName);   
    }
}
