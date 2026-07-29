using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Source;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public class SourceMetadataService : LoggingServiceBase<SourceMetadataService>, ISourceMetadataService
    {
        private readonly IMetadataRepository _metadataRepo;
        public SourceMetadataService(IMetadataRepository metadataRepo, ILogger<SourceMetadataService> logger)
            : base(logger)
        {
            _metadataRepo = metadataRepo;   
        }

        public List<string> GetSynchronizedTableNames()
        {
            throw new NotImplementedException();
        }

        public List<Table>? GetTables()
        {
            LogIfEnabled(LogLevel.Information, "Getting tables from source metadata service...");
            try
            {
                var tables = _metadataRepo.GetTables();
                LogIfEnabled(LogLevel.Information, "Successfully retrieved {Count} table.", tables?.Count);
                return tables; 

            }
            catch(ReadMetadataException ex)
            {
                var message = "An error occurred while reading metadata for tables.";
                LogIfEnabled(LogLevel.Error, ex, message);
                throw new ApplicationServiceException(message, ex);
            }
        }
    }
}
