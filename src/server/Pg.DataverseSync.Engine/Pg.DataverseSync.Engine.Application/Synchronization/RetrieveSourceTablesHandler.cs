using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Synchronization
{
    internal sealed class RetrieveSourceTablesHandler : SyncMetadataHandler
    {
        private readonly ISourceMetadataService _sourceMetadataService;

        public RetrieveSourceTablesHandler(ISourceMetadataService sourceMetadataService)
        {
            _sourceMetadataService = sourceMetadataService;
        }

        protected override void Execute(SyncMetadataExecutionContext context)
        {
            try
            {
                context.SourceTables = _sourceMetadataService.GetTables() ?? new List<Table>();
            }
            catch (Exception ex)
            {
                throw new ApplicationServiceException(
                    "An error occurred while retrieving source tables metadata.", ex);
            }
        }
    }
}
