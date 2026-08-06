using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;

namespace Pg.DataverseSync.Engine.Source
{
    public abstract class DataRepositoryBase : LoggingServiceBase<DataRepositoryBase>
    {
        protected readonly IOrganizationService service;

        protected DataRepositoryBase(IOrganizationService service, ILogger<DataRepositoryBase> logger)
            : base(logger)
        {
            this.service = service;
        }
    }
}
