using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;


namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class ServiceBusEndpointsRepository : DataRepository, IServiceBusEndpointsRepository
    {
        public ServiceBusEndpointsRepository(IOrganizationServiceFactory orgSvcFactory) : base(orgSvcFactory)
        {
        }

        public void CreateStep(SdkMessageProcessingStep step, string entityName)
        {
            if (StepExists(step.EventHandler.Id, step.SdkMessageFilterId.ToString(), entityName))
            {
                throw new InvalidOperationException
                    ($"Step already exists for the given entity {entityName} and message filter.");
            }

            service.Create(step);
        }

        public void DeleteStep(Guid serviceEndpointId, string messageName, string entityName)
        {
            var messageFilterId = GetSdkMessageFilterId(messageName, entityName);
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
            var results = service.RetrieveMultiple(query);
            foreach (var step in results.Entities)
            {
                service.Delete(SdkMessageProcessingStep.EntityLogicalName, step.Id);
            }
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

            return service.RetrieveMultiple(query).Entities.Count > 0;
        }
    }
}
