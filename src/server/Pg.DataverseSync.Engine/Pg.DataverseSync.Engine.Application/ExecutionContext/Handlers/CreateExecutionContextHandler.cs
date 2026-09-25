using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Create' execution context messages.
    /// </summary>
    public class CreateExecutionContextHandler : ExecutionContextHandlerBase<CreateExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Create;

        public CreateExecutionContextHandler(ITargetDataRepository targetDataRepository, 
            ITargetRecordFactory targetRecordFactory, ILogger<CreateExecutionContextHandler> logger) 
            : base(targetDataRepository, targetRecordFactory, logger)
        {
        }
  
        public async Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            LogIfEnabled(LogLevel.Information,
                "Processing Create execution context (CorrelationId: {CorrelationId})",
                context.CorrelationId);

            if (!context.InputParameters.TryGetValue(ParameterNames.Target, out var targetEntity))
            {
                throw new InvalidOperationException($"{MessageNames.Create} message missing '{ParameterNames.Target}' in InputParameters.");
            }

            var entity = (Entity)targetEntity;

            try
            {
                LogIfEnabled(LogLevel.Information,
                    "Create handler: Entity LogicalName={LogicalName}, Id={Id}",
                    entity.LogicalName,
                    entity.Id);

                var targetRecord = targetRecordFactory.CreateForInsert(entity); 
                var result = targetDataRepository.InsertRecord(targetRecord);

                HandleExecutionResult(result, entity.ToEntityReference(), MessageName);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, "Error in CreateExecutionContextHandler: {Message}", ex.Message);
                throw new ExecutionContextHandlerException(MessageName, entity.Id, entity.LogicalName, ex);
            }
        }
    }
}
