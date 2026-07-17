using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Create' execution context messages.
    /// </summary>
    public class CreateExecutionContextHandler : IExecutionContextHandler
    {
        public string MessageName => MessageNames.Create;

        private readonly ILogger<CreateExecutionContextHandler> _logger;

        public CreateExecutionContextHandler(ILogger<CreateExecutionContextHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context); 

            _logger.LogInformation(
                "Processing Create execution context (CorrelationId: {correlationId})",
                context.CorrelationId);

            if (!context.InputParameters.TryGetValue(ParameterNames.Target, out var targetEntity))
            {
                throw new InvalidOperationException($"{MessageNames.Create} message missing '{ParameterNames.Target}' in InputParameters.");
            }

            var entity = (Entity)targetEntity;

            try
            {

                _logger.LogInformation(
                    "Create handler: Entity LogicalName={logicalName}, Id={id}",
                    entity.LogicalName,
                    entity.Id);

                // TODO: Add business logic for handling Create message

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateExecutionContextHandler");
                throw new ExecutionContextHandlerException(MessageName, entity.Id, entity.LogicalName);
            }
        }
    }
}
