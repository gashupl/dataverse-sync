using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Application.Source;

namespace Pg.DataverseSync.Engine.Source
{
    public class DataRepository : DataverseRepositoryBase, IDataRepository
    {
        public DataRepository(IOrganizationService service, ILogger<DataverseRepositoryBase> logger) 
            : base(service, logger)
        {
        }

        public List<Entity> GetRecords(string tableName, List<string> columns)
        {
            var records = new List<Entity>();
            var query = new QueryExpression(tableName)
            {
                ColumnSet = new ColumnSet(columns.ToArray()), 
                PageInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 5000
                }
            };

            EntityCollection results;
            do
            {
                results = service.RetrieveMultiple(query);

                foreach (var entity in results.Entities)
                {
                    var newEntity = new Entity(tableName);
                    foreach (var attr in entity.Attributes)
                    {
                        if (columns.Contains(attr.Key))
                        {
                            newEntity.Attributes.Add(attr);
                        }
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
