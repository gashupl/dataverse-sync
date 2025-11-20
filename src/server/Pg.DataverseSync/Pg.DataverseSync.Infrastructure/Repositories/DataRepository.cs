using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using Pg.DataverseSync.Domain.Repositories;
using System.Web.UI.WebControls;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class DataRepository : IRepository
    {
        private readonly IOrganizationService _service;

        public DataRepository(IOrganizationServiceFactory orgSvcFactory)
        {
            _service = orgSvcFactory.CreateOrganizationService(null); 
        }

        public List<pg_synctable> GetSynchronizedTables()
        {
            throw new NotImplementedException();
        }

        public List<Table> GetTablesFromMetadata()
        {
            throw new NotImplementedException();
        }

    }
}

