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
    public class CreateExecutionContextHandler : LoggingServiceBase<CreateExecutionContextHandler>, IExecutionContextHandler
    {
        private readonly ITargetDataRepository _targetDataRepository;

        public string MessageName => MessageNames.Create;
        
        private readonly ITargetRecordFactory _targetRecordFactory;

        public CreateExecutionContextHandler(ITargetDataRepository targetDataRepository, 
            ITargetRecordFactory targetRecordFactory, ILogger<CreateExecutionContextHandler> logger) : base(logger)
        {
            _targetDataRepository = targetDataRepository;
            _targetRecordFactory = targetRecordFactory;
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

                var targetRecord = _targetRecordFactory.CreateForInsert(entity); 
                var result = _targetDataRepository.InsertRecord(targetRecord);

                if (!result.Success)
                {
                    LogIfEnabled(LogLevel.Error,
                        "Insert record failed for entity LogicalName={LogicalName}, Id={Id}. Error: {ErrorMessage}",
                        entity.LogicalName,
                        entity.Id,
                        result.Message!);
                    throw new InvalidOperationException($"Failed to insert record for entity '{entity.LogicalName}' with id '{entity.Id}': {result.Message}");
                }

                LogIfEnabled(LogLevel.Information,
                    "Record successfully inserted for entity LogicalName={LogicalName}, Id={Id}",
                    entity.LogicalName,
                    entity.Id);

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
