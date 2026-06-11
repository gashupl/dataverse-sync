using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System.Collections.Generic;
using System.Linq;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class SyncTablesRepository : DataRepository, ISyncTablesRepository
    {
        public SyncTablesRepository(IOrganizationServiceFactory orgSvcFactory) : base(orgSvcFactory)
        {
        }

        public List<pg_synctable> GetActiveSynchronizedTables()
        {
            using (var context = new DataverseContext(service))
            {
                var query = context.pg_synctableSet
                    .Where(st => st.StateCode == pg_synctable_statecode.Active)
                    .Select(st => new pg_synctable
                    {
                        Id = st.Id,
                        pg_name = st.pg_name,
                    });
                return query.ToList<pg_synctable>();
            }
        }
    }
}
