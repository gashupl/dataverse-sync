using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;

namespace Pg.DataverseSync.Domain.Services
{
    public class EndpointStepCreationService : ServiceBase, IEndpointStepCreationService
    {
        private readonly Guid _serviceEndpointId;

        public EndpointStepCreationService(IRepository repository, ITracingService tracingService, Guid serviceEndpointId) : base(repository, tracingService)
        {
            _serviceEndpointId = serviceEndpointId;
        }

        public EndpointStepCreationResult CreateStepForEntity(string entityName, string messageName)
        {
            try
            {
                var sdkMessageId = repository.GetSdkMessageId(messageName);
                var sdkMessageFilterId = repository.GetSdkMessageFilterId(messageName, entityName);
                if(sdkMessageFilterId == null)
                {
                    tracingService.Trace($"No SDK Message Filter found for message '{messageName}' and entity '{entityName}'. Step creation aborted.");
                    return new EndpointStepCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"No SDK Message Filter found for message '{messageName}' and entity '{entityName}'."
                    };
                }

                var step = new SdkMessageProcessingStep
                {
                    SdkMessageId = new EntityReference(SdkMessage.EntityLogicalName, sdkMessageId),
                    SdkMessageFilterId = new EntityReference(SdkMessageFilter.EntityLogicalName, sdkMessageFilterId.Value),
                    EventHandler = new EntityReference("serviceendpoint", _serviceEndpointId),
                    Name = $"DataverseSync Endpoint: {messageName} to {entityName}",
                    Rank = 1,
                    Description = $"DataverseSync Endpoint: {messageName} to {entityName}",
                    Stage = SdkMessageProcessingStep_Stage.PostOperation,
                    Mode = SdkMessageProcessingStep_Mode.Asynchronous
                };

                repository.CreateStep(step, entityName);
                return new EndpointStepCreationResult { Success = true };
            }
            catch (Exception ex)
            {
                tracingService.Trace($"CreateStepForEntity failed for entity '{entityName}' and message '{messageName}': {ex.Message}");
                return new EndpointStepCreationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
