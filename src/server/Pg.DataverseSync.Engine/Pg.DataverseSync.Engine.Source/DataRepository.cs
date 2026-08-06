using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Source
{
    public class DataRepository : DataRepositoryBase, IDataRepository
    {
        public DataRepository(IOrganizationService service, ILogger<DataRepository> logger) 
            : base(service, logger)
        {
        }

        public List<Entity> GetActiveSyncTables()
        {
            LogIfEnabled(LogLevel.Information, "GetActiveSyncTables method executed");

            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(SyncTable.Columns.StateCode, 
                ConditionOperator.Equal, SyncTable.StateCodes.Active);

            var syncTables = this.GetRecords(SyncTable.EntityName, new List<string> { SyncTable.Columns.Name },
                filter);

            LogIfEnabled(LogLevel.Information, "GetActiveSyncTables method completed");
            return syncTables;
        }

        public List<Entity> GetRecords(string tableName, List<string> columns, FilterExpression? filter)
        {
            var records = new List<Entity>();
            var query = new QueryExpression(tableName)
            {
                ColumnSet = new ColumnSet(columns.ToArray()), 
                Criteria = filter, 
                PageInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 5000
                },
            };

            EntityCollection results;
            do
            {
                results = service.RetrieveMultiple(query);

                foreach (var entity in results.Entities)
                {
                    var newEntity = new Entity(tableName);
                    foreach (var attr in entity.Attributes.Where(attr => columns.Contains(attr.Key)))
                    {
                        newEntity.Attributes.Add(attr);
                    }
                    records.Add(newEntity);
                }

                // Increment the page number to retrieve the next set of records
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = results.PagingCookie;

            }
            while (results.MoreRecords);

            return records;
        }

    }
}
