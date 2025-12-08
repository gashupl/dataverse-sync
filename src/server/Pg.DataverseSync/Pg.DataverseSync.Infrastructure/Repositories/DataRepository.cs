using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using Pg.DataverseSync.Domain.Repositories;
using System.Linq;
using Pg.DataverseSync.Domain.Dto;
using Microsoft.Xrm.Sdk.Metadata;
using Pg.DataverseSync.Infrastructure.Core;

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

        public List<Table> GetStandardTablesFromMetadata()
        {
            var tables = new List<Table>();

            var request = new Microsoft.Xrm.Sdk.Messages.RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var response = (Microsoft.Xrm.Sdk.Messages.RetrieveAllEntitiesResponse)_service.Execute(request);

            foreach (var entity in response.EntityMetadata)
            {
                // Exclude Virtual and Elastic tables
                if (entity.TableType == TableTypes.Standard)
                {
                    tables.Add(new Table
                    {
                        Name = entity.DisplayName?.UserLocalizedLabel?.Label ?? entity.LogicalName,
                        SchemaName = entity.LogicalName
                    });
                }
            }

            return tables;
        }

    }
}

