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

            //TODO: Retrieve list of synchronized tables using IDataRepository
            //TODO: Retrieve list of tables from source using ISourceMetadataService
            //TODO: For every synchronized table, check if it exists in the target schema using ITargetSchemaService
            //TODO: If table does not exist in the target schema, craete it in the target schema
            //TODO: If table exists in the target schema, check if the schema is up to date using ITargetSchemaService
            return new SyncMetadataResult();
        }
    }
}
