using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;

namespace Pg.DataverseSync.Domain.Services
{
    public class EndpointStepCreationService : ServiceBase, IEndpointStepCreationService
    {
        private const string _serviceEndpointIdEnvVariableName = "pg_dataversesyncendpointid";
        private readonly IEnvironmentVariablesRepository _envVariablesRepository;
        private readonly IServiceBusEndpointsRepository _serviceBusEndpointsRepository;

        public EndpointStepCreationService(IEnvironmentVariablesRepository envVariablesRepository, IServiceBusEndpointsRepository serviceBusEndpointsRepository, ITracingService tracingService) 
            : base(tracingService)
        {
            _envVariablesRepository = envVariablesRepository;
            _serviceBusEndpointsRepository = serviceBusEndpointsRepository;
        }

        public EndpointStepCreationResult CreateStepForEntity(string entityName, string messageName)
        {
            try
            {
                var parseResult 
                    = Guid.TryParse(_envVariablesRepository.GetValue(_serviceEndpointIdEnvVariableName), out var serviceEndpointId); 
                
                if (!parseResult)
                {
                    tracingService.Trace("Missing or invalid ServiceEndpointId. Step creation aborted.");
                    return new EndpointStepCreationResult
                    {
                        Success = false,
                        ErrorMessage = "Missing or invalid ServiceEndpointId."
                    };
                }

                var sdkMessageId = _serviceBusEndpointsRepository.GetSdkMessageId(messageName);
                var sdkMessageFilterId = _serviceBusEndpointsRepository.GetSdkMessageFilterId(messageName, entityName);
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
                    EventHandler = new EntityReference("serviceendpoint", serviceEndpointId),
                    Name = $"DataverseSync Endpoint: {messageName} to {entityName}",
                    Rank = 1,
                    Description = $"DataverseSync Endpoint: {messageName} to {entityName}",
                    Stage = SdkMessageProcessingStep_Stage.PostOperation,
                    Mode = SdkMessageProcessingStep_Mode.Asynchronous
                };

                _serviceBusEndpointsRepository.CreateStep(step, entityName);
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
