using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;

namespace Pg.DataverseSync.Domain.Services
{
    public abstract class ServiceBase
    {
        protected readonly IRepository repository;
        protected readonly ITracingService tracingService;

        public ServiceBase(IRepository repository, ITracingService tracingService)
        {
            this.repository = repository;
            this.tracingService = tracingService;
        }
    }
}

