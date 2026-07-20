using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Source;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public class SourceMetadataService : ISourceMetadataService
    {
        private readonly IMetadataReader _metadataReader;
        private readonly ILogger _logger;
        public SourceMetadataService(IMetadataReader metadataReader, ILogger<SourceMetadataService> logger)
        {
            _metadataReader = metadataReader;
            _logger = logger;
        }

        public List<Table>? GetTables()
        {
            _logger.LogInformation("Getting tables from source metadata service...");
            try
            {
                var tables = _metadataReader.GetTables();
                _logger.LogInformation("Successfully retrieved {Count} table.", tables?.Count);
                return tables; 

            }
            catch(ReadMetadataException ex)
            {
                var message = "An error occurred while reading metadata for tables.";
                _logger.LogError(ex, message);
                throw new ApplicationServiceException(message, ex);
            }
        }
    }
}
