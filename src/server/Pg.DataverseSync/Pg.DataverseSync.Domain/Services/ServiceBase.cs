using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;

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

