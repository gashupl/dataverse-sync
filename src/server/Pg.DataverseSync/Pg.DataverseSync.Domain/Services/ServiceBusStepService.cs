using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;
using System.Web.UI.WebControls;

namespace Pg.DataverseSync.Domain.Services
{
    public class ServiceBusStepService : ServiceBase, IServiceBusStepService
    {
        private const string _serviceEndpointIdEnvVariableName = "pg_dataversesyncendpointid";
        private readonly IEnvironmentVariablesRepository _envVariablesRepository;
        private readonly IServiceBusEndpointsRepository _serviceBusEndpointsRepository;

        public ServiceBusStepService(IEnvironmentVariablesRepository envVariablesRepository, IServiceBusEndpointsRepository serviceBusEndpointsRepository, ITracingService tracingService) 
            : base(tracingService)
        {
            _envVariablesRepository = envVariablesRepository;
            _serviceBusEndpointsRepository = serviceBusEndpointsRepository;
        }

        public ServiceOperationResult CreateStepsForEntity(string entityName, string[] messageNames)
        {
            try
            {
                var parseResult 
                    = Guid.TryParse(_envVariablesRepository.GetValue(_serviceEndpointIdEnvVariableName), out var serviceEndpointId); 

                if (!parseResult)
                {
                    tracingService.Trace("Missing or invalid ServiceEndpointId. Step creation aborted.");
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Missing or invalid ServiceEndpointId."
                    };
                }

                foreach (var messageName in messageNames)
                {
                    var sdkMessageId = _serviceBusEndpointsRepository.GetSdkMessageId(messageName);
                    var sdkMessageFilterId = _serviceBusEndpointsRepository.GetSdkMessageFilterId(messageName, entityName);
                    if (sdkMessageFilterId == null)
                    {
                        tracingService.Trace($"No SDK Message Filter found for message '{messageName}' and entity '{entityName}'. Step creation aborted.");
                        return new ServiceOperationResult
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

                    tracingService.Trace("Creating step for message '{0}' and entity '{1}' with ServiceEndpointId '{2}'", messageName, entityName, serviceEndpointId);
                    _serviceBusEndpointsRepository.CreateStep(step, messageName, entityName);
                    tracingService.Trace("Step created successfully for message '{0}' and entity '{1}'", messageName, entityName);
                }

                return new ServiceOperationResult { Success = true };
            }
            catch (Exception ex)
            {
                tracingService.Trace($"CreateStepsForEntity failed for entity '{entityName}': {ex.Message}");
                return new ServiceOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult DeleteStepsForEntity(string entityName, string[] messageNames)
        {
            try
            {
                var parseResult
                    = Guid.TryParse(_envVariablesRepository.GetValue(_serviceEndpointIdEnvVariableName), out var serviceEndpointId);

                if (!parseResult)
                {
                    tracingService.Trace("Missing or invalid ServiceEndpointId. Step deletion aborted.");
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Missing or invalid ServiceEndpointId."
                    };
                }

                foreach (var messageName in messageNames)
                {
                    _serviceBusEndpointsRepository.DeleteStep(serviceEndpointId, messageName, entityName);
                }

                return new ServiceOperationResult { Success = true };
            }
            catch (Exception ex)
            {
                tracingService.Trace($"DeleteStepsForEntity failed for entity '{entityName}': {ex.Message}");
                return new ServiceOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
