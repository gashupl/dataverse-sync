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

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class DataRepository : IRepository
    {
        private readonly IOrganizationService _service;

        public DataRepository(IOrganizationServiceFactory orgSvcFactory)
        {
            _service = orgSvcFactory.CreateOrganizationService(null); 
        }

        public void CreateStep(SdkMessageProcessingStep step, string entityName)
        {
            if(StepExists(step.EventHandler.Id, step.SdkMessageFilterId.ToString(), entityName))
            {
                throw new InvalidOperationException
                    ($"Step already exists for the given entity {entityName} and message filter.");
            }

            _service.Create(step); 
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

        public bool StepExists(Guid serviceEndpointId, string messageName, string entityName)
        {
            var messageFilterId = GetSdkMessageFilterId(messageName, entityName);

            if (messageFilterId == null)
                return false;

            var query = new QueryExpression(SdkMessageProcessingStep.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SdkMessageProcessingStep.Fields.SdkMessageProcessingStepId),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageProcessingStep.Fields.EventHandler, 
                            ConditionOperator.Equal, 
                            serviceEndpointId),
                        new ConditionExpression(SdkMessageProcessingStep.Fields.SdkMessageFilterId, 
                            ConditionOperator.Equal, 
                            messageFilterId.Value)
                    }
                }
            };

            return _service.RetrieveMultiple(query).Entities.Count > 0;
        }


        private Guid? GetSdkMessageFilterId(string messageName, string entityName)
        {
            var query = new QueryExpression(SdkMessageFilter.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SdkMessageFilter.Fields.SdkMessageFilterId, SdkMessageFilter.Fields.PrimaryObjectTypeCode),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageFilter.Fields.PrimaryObjectTypeCode, 
                            ConditionOperator.Equal, 
                            entityName)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(SdkMessageFilter.EntityLogicalName, 
                        SdkMessage.EntityLogicalName, 
                        SdkMessageFilter.Fields.SdkMessageId, 
                        SdkMessage.Fields.SdkMessageId, 
                        JoinOperator.Inner)
                    {
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression(SdkMessage.Fields.Name, 
                                    ConditionOperator.Equal, 
                                    messageName)
                            }
                        }
                    }
                }
            };

            var results = _service.RetrieveMultiple(query);
            return results.Entities.Count > 0 ? results.Entities[0].Id : (Guid?)null;
        }

    }
}

