using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Update' execution context messages.
    /// </summary>
    public class UpdateExecutionContextHandler : LoggingServiceBase<UpdateExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Update;

        private readonly ITargetDataRepository _targetDataRepository;
        private readonly ITargetRecordFactory _targetRecordFactory;

        public UpdateExecutionContextHandler(ITargetDataRepository targetDataRepository,
            ITargetRecordFactory targetRecordFactory, ILogger<UpdateExecutionContextHandler> logger) : base(logger)
        {
            _targetDataRepository = targetDataRepository;
            _targetRecordFactory = targetRecordFactory;
        }

        public async Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            LogIfEnabled(LogLevel.Information,
                "Processing Update execution context (CorrelationId: {CorrelationId})",
                context.CorrelationId);

            if (!context.InputParameters.TryGetValue(ParameterNames.Target, out var targetEntity))
            {
                throw new InvalidOperationException($"{MessageNames.Update} message missing '{ParameterNames.Target}' in InputParameters.");
            }

            var entity = (Entity)targetEntity;

            try
            {
                LogIfEnabled(LogLevel.Information,
                    "Update handler: Entity LogicalName={LogicalName}, Id={Id}",
                    entity.LogicalName,
                    entity.Id);

                var targetRecord = _targetRecordFactory.CreateForUpdate(entity);
                var result = _targetDataRepository.UpdateRecord(targetRecord);

                if (!result.Success)
                {
                    LogIfEnabled(LogLevel.Error,
                        "Update record failed for entity LogicalName={LogicalName}, Id={Id}. Error: {ErrorMessage}",
                        entity.LogicalName,
                        entity.Id,
                        result.Message!);
                    throw new InvalidOperationException($"Failed to update record for entity '{entity.LogicalName}' with id '{entity.Id}': {result.Message}");
                }

                LogIfEnabled(LogLevel.Information,
                    "Record successfully updated for entity LogicalName={LogicalName}, Id={Id}",
                    entity.LogicalName,
                    entity.Id);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, "Error in UpdateExecutionContextHandler: {Message}", ex.Message);
                throw new ExecutionContextHandlerException(MessageName, entity.Id, entity.LogicalName, ex);
            }
        }
    }
}
