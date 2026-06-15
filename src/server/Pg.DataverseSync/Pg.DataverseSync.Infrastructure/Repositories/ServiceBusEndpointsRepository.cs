using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;


namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class ServiceBusEndpointsRepository : DataRepository, IServiceBusEndpointsRepository
    {
        public ServiceBusEndpointsRepository(IOrganizationServiceFactory orgSvcFactory, ITracingService tracingService) 
            : base(orgSvcFactory, tracingService)
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

            using (var context = new DataverseContext(service))
            {
                var stepIds = BuildSqkMessageProcessingStepQuery(context, serviceEndpointId, messageFilterId)
                    .ToList();

                foreach (var stepId in stepIds)
                {
                    service.Delete(SdkMessageProcessingStep.EntityLogicalName, stepId.Value);
                }
            }
        }

        public bool StepExists(Guid serviceEndpointId, string messageName, string entityName)
        {
            var messageFilterId = GetSdkMessageFilterId(messageName, entityName);

            if (messageFilterId == null)
                return false;

            using (var context = new DataverseContext(service))
            {
                return BuildSqkMessageProcessingStepQuery(context, serviceEndpointId, messageFilterId)
                    .FirstOrDefault() != null;
            }
        }

        private IQueryable<Guid?> BuildSqkMessageProcessingStepQuery(
            DataverseContext context, Guid serviceEndpointId, Guid? messageFilterId)
        {
            return context.SdkMessageProcessingStepSet
                    .Where(s => s.EventHandler.Id == serviceEndpointId
                             && s.SdkMessageFilterId.Id == messageFilterId.Value)
                    .Select(s => s.SdkMessageProcessingStepId); 
        }

    }
}
