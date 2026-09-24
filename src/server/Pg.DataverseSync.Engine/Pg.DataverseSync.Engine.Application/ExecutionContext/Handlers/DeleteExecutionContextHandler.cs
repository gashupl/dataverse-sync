using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Delete' execution context messages.
    /// </summary>
    public class DeleteExecutionContextHandler : LoggingServiceBase<DeleteExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Delete;

        private readonly ITargetDataRepository _targetDataRepository;
        private readonly ITargetRecordFactory _targetRecordFactory;

        public DeleteExecutionContextHandler(ITargetDataRepository targetDataRepository,
            ITargetRecordFactory targetRecordFactory, ILogger<DeleteExecutionContextHandler> logger) : base(logger)
        {
            _targetDataRepository = targetDataRepository;
            _targetRecordFactory = targetRecordFactory; 
        }

        public async Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            LogIfEnabled(LogLevel.Information,
                "Processing Delete execution context (CorrelationId: {CorrelationId})",
                context.CorrelationId);

            if (!context.InputParameters.TryGetValue(ParameterNames.Target, out var targetRef))
            {
                throw new InvalidOperationException($"{MessageNames.Delete} message missing '{ParameterNames.Target}' in InputParameters.");
            }

            var entityRef = (EntityReference)targetRef;

            try
            {
                LogIfEnabled(LogLevel.Information,
                    "Delete handler: Entity LogicalName={LogicalName}, Id={Id}",
                    entityRef.LogicalName,
                    entityRef.Id);

                var targetRecord = _targetRecordFactory.CreateForDelete(entityRef);
                var result = _targetDataRepository.DeleteRecord(targetRecord);

                if (!result.Success)
                {
                    LogIfEnabled(LogLevel.Error,
                        "Delete record failed for entity LogicalName={LogicalName}, Id={Id}. Error: {ErrorMessage}",
                        entityRef.LogicalName,
                        entityRef.Id,
                        result.Message!);
                    throw new InvalidOperationException($"Failed to delete record for entity '{entityRef.LogicalName}' with id '{entityRef.Id}': {result.Message}");
                }

                LogIfEnabled(LogLevel.Information,
                    "Record successfully deleted for entity LogicalName={LogicalName}, Id={Id}",
                    entityRef.LogicalName,
                    entityRef.Id);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, "Error in DeleteExecutionContextHandler: {Message}", ex.Message);
                throw new ExecutionContextHandlerException(MessageName, entityRef.Id, entityRef.LogicalName, ex);
            }
        }
    }
}
