using Pg.DataverseSync.Model;
using System;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IServiceBusEndpointsRepository : IRepository
    {
        void CreateStep(SdkMessageProcessingStep step, string entityName);

        bool StepExists(Guid serviceEndpointId, string messageName, string entityName);

        void DeleteStep(Guid serviceEndpointId, string messageName, string entityName);
    }
}
