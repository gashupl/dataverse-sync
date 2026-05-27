using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Infrastructure.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class DataRepository : IRepository
    {
        private readonly IOrganizationService _service;

        public DataRepository(IOrganizationServiceFactory orgSvcFactory)
        {
            _service = orgSvcFactory.CreateOrganizationService(null); 
        }

        public void CreateStep(SdkMessageProcessingStep step)
        {
            throw new NotImplementedException();
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

            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var a = _service.Execute(request); 
            var response = (RetrieveAllEntitiesResponse)a;

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

        private bool StepAlreadyExists(Guid serviceEndpointId, string messageName, string entityName)
        {
            var messageFilterId = GetSdkMessageFilterId(messageName, entityName);

            if (messageFilterId == null)
                return false; 

            using (var context = new DataverseContext(_service))
            {
                return context.SdkMessageProcessingStepSet
                    .Where(st => st.EventHandler.Id == serviceEndpointId
                              && st.EventHandler.LogicalName == "serviceendpoint"
                              && st.SdkMessageFilterId.Id == messageFilterId)
                    .Any();
            }
        }


        private Guid? GetSdkMessageFilterId(string messageName, string entityName)
        {
            using (var context = new DataverseContext(_service))
            {
                var filter = context.SdkMessageFilterSet
                    .Where(f => f.PrimaryObjectTypeCode == entityName
                             && f.SdkMessageIdName == messageName)
                    .Select(f => new SdkMessageFilter { Id = f.Id })
                    .FirstOrDefault();

                return filter?.Id;
            }
        }

    }
}

