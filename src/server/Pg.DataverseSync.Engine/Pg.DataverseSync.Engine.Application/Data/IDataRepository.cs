using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface IDataRepository
    {
        List<Entity> GetActiveSyncTables(); 
        List<Entity> GetRecords(string tableName, List<string> columns, FilterExpression? filter); 
    }
}
