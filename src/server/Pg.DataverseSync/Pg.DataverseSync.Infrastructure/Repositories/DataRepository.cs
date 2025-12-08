using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using Pg.DataverseSync.Domain.Repositories;
using System.Web.UI.WebControls;
using System.Linq;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class DataRepository : IRepository
    {
        private readonly IOrganizationService _service;

        public DataRepository(IOrganizationServiceFactory orgSvcFactory)
        {
            _service = orgSvcFactory.CreateOrganizationService(null); 
        }

        public List<pg_synctable> GetActiveSynchronizedTables()
        {
            using (var context = new DataverseContext(_service))
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

        public List<Table> GetTablesFromMetadata()
        {
            throw new NotImplementedException();
        }

    }
}

