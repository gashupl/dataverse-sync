using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Synchronization
{
    internal sealed class CheckTargetSchemaHandler : SyncMetadataHandler
    {
        private readonly ITargetSchemaService _targetSchemaService;

        public CheckTargetSchemaHandler(ITargetSchemaService targetSchemaService)
        {
            _targetSchemaService = targetSchemaService;
        }

        protected override void Execute(SyncMetadataExecutionContext context)
        {
            try
            {
                foreach (var synchronizedTableName in context.SynchronizedTableNames)
                {
                    context.TargetTableExists[synchronizedTableName] =
                        _targetSchemaService.TargetTableExists(synchronizedTableName);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationServiceException(
                    "An error occurred while checking target schema for synchronized tables.", ex);
            }
        }
    }
}
