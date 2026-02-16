using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Domain.Source;

namespace Pg.DataverseSync.Engine.Domain
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

        public List<string> GetTablesNames()
        {
            _logger.LogInformation("Getting table names from source metadata service...");
            try
            {
                var tables = _metadataReader.GetTables();
                _logger.LogInformation("Successfully retrieved {Count} table names.", tables?.Count);
                return tables?.Select(t => t.Name)?.ToList() ?? new List<string>();

            }
            catch(ReadMetadataException ex)
            {
                var message = "An error occurred while reading metadata for table names.";
                _logger.LogError(ex, message);
                throw new DomainServiceException(message, ex);
            }
        }
    }
}
