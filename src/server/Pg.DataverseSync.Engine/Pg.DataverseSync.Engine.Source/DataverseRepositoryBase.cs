using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Source
{
    public abstract class DataverseRepositoryBase : LoggingServiceBase<DataverseRepositoryBase>
    {
        protected readonly IOrganizationService service;

        public DataverseRepositoryBase(IOrganizationService service, ILogger<DataverseRepositoryBase> logger)
            : base(logger)
        {
            this.service = service;
        }
    }
}
