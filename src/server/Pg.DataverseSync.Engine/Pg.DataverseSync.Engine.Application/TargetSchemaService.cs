using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Source;

namespace Pg.DataverseSync.Engine.Application
{
    public class TargetSchemaService : LoggingServiceBase<TargetSchemaService>, ITargetSchemaService
    {
        private readonly ITargetSchemaRepository _schemaRepository;

        public TargetSchemaService(ITargetSchemaRepository schemaRepository, ILogger<TargetSchemaService> logger) : base(logger)
        {
            _schemaRepository = schemaRepository;
        }
    }
}
