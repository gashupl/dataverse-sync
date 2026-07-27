using Microsoft.Extensions.Logging;

namespace Pg.DataverseSync.Engine.Application
{
    public class SyncMetadataService : LoggingServiceBase<SyncMetadataService>, ISyncMetadataService
    {
        public SyncMetadataService(ILogger<SyncMetadataService> logger) : base(logger)
        {
        }

        public SyncMetadataResult Execute()
        {
            // Implementation of the Execute method
            return new SyncMetadataResult();
        }
    }
}
