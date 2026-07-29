using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Synchronization
{
    internal sealed class RetrieveSynchronizedTablesHandler : SyncMetadataHandler
    {
        private readonly ISourceMetadataService _sourceMetadataService;

        public RetrieveSynchronizedTablesHandler(ISourceMetadataService sourceMetadataService)
        {
            _sourceMetadataService = sourceMetadataService;
        }

        protected override void Execute(SyncMetadataExecutionContext context)
        {
            try
            {
                context.SynchronizedTableNames = _sourceMetadataService.GetSynchronizedTableNames()
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationServiceException(
                    "An error occurred while retrieving synchronized table names.", ex);
            }
        }
    }
}
