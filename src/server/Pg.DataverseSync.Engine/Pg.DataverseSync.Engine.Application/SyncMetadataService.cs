using Microsoft.Extensions.Logging;

namespace Pg.DataverseSync.Engine.Application
{
    public class SyncMetadataService : LoggingServiceBase<SyncMetadataService>, ISyncMetadataService
    {
        private readonly ISourceMetadataService _sourceMetadataService;
        private readonly ITargetSchemaService _targetSchemaService;

        public SyncMetadataService(ISourceMetadataService sourceMetadataService,
            ITargetSchemaService targetSchemaService,
            ILogger<SyncMetadataService> logger) : base(logger)
        {
            _sourceMetadataService = sourceMetadataService;
            _targetSchemaService = targetSchemaService;
        }

        public SyncMetadataResult Execute()
        {
            // Implementation of the Execute method
            return new SyncMetadataResult();
        }
    }
}
