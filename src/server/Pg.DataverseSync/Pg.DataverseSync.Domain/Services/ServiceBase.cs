using Microsoft.Xrm.Sdk;

namespace Pg.DataverseSync.Domain.Services
{
    public abstract class ServiceBase
    {
        protected readonly ITracingService tracingService;

        public ServiceBase(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }
    }
}

