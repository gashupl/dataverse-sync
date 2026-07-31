using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Application
{
    public class SourceMetadataService : LoggingServiceBase<SourceMetadataService>, ISourceMetadataService
    {
        private readonly IMetadataRepository _metadataRepo;
        private readonly IDataRepository _dataRepository; 

        public SourceMetadataService(IMetadataRepository metadataRepo, 
            IDataRepository dataRepository, ILogger<SourceMetadataService> logger)
            : base(logger)
        {
            _metadataRepo = metadataRepo;   
            _dataRepository = dataRepository;
        }

        public List<string> GetSynchronizedTableNames()
        {
            var names = _dataRepository
                .GetActiveSyncTables().Select(t => t.Attributes[SyncTable.Columns.Name] as string)
                .Where(n => n != null);

            return names.ToList()!; 
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
