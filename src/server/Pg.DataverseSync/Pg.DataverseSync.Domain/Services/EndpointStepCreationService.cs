using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using System;

namespace Pg.DataverseSync.Domain.Services
{
    public class EndpointStepCreationService : ServiceBase, IEndpointStepCreationService
    {
        public EndpointStepCreationService(IRepository repository, ITracingService tracingService) : base(repository, tracingService)
        {
        }
        public EndpointStepCreationResult CreateStepForEntity(string entityName)
        {
            try
            {
                var step = new Model.SdkMessageProcessingStep();
                repository.CreateStep(step, entityName);
                return new EndpointStepCreationResult { Success = true };
            }
            catch (Exception ex)
            {
                tracingService.Trace($"CreateStepForEntity failed for entity '{entityName}': {ex.Message}");
                return new EndpointStepCreationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
