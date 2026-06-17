using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Infrastructure.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class DataRepository : IRepository
    {
        protected readonly IOrganizationService service;
        protected readonly ITracingService tracingService;

        public DataRepository(IOrganizationServiceFactory orgSvcFactory, ITracingService tracingService)
        {
            service = orgSvcFactory.CreateOrganizationService(null); 
            this.tracingService = tracingService;
        }

        public List<Table> GetStandardTablesFromMetadata()
        {
            var tables = new List<Table>();

            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var a = service.Execute(request); 
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


        public Guid GetSdkMessageId(string messageName)
        {
            using (var context = new DataverseContext(service))
            {
                var messageId = context.SdkMessageSet
                    .Where(m => m.Name == messageName)
                    .Select(m => m.SdkMessageId)
                    .FirstOrDefault();

                if (messageId == null)
                    throw new InvalidOperationException($"SDK Message '{messageName}' not found.");

                return messageId.Value;
            }
        }

        public Guid? GetSdkMessageFilterId(string messageName, string entityName)
        {
            using (var context = new DataverseContext(service))
            {
                var filterId = (from f in context.SdkMessageFilterSet
                                join m in context.SdkMessageSet
                                on f.SdkMessageId.Id equals m.Id
                                where f.PrimaryObjectTypeCode == entityName
                                   && m.Name == messageName
                                select f.SdkMessageFilterId).FirstOrDefault();

                return filterId;
            }
        }


    }
}

