using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Delete' execution context messages.
    /// </summary>
    public class DeleteExecutionContextHandler : IExecutionContextHandler
    {
        public string MessageName => "Delete";

        private readonly ILogger<DeleteExecutionContextHandler> _logger;

        public DeleteExecutionContextHandler(ILogger<DeleteExecutionContextHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            try
            {
                _logger.LogInformation(
                    "Processing Delete execution context (CorrelationId: {correlationId})",
                    context.CorrelationId);

                if (!context.InputParameters.TryGetValue("Target", out var targetRef))
                {
                    throw new InvalidOperationException("Delete message missing 'Target' in InputParameters.");
                }

                var entity = (EntityReference)targetRef;
                _logger.LogInformation(
                    "Delete handler: Entity LogicalName={logicalName}, Id={id}",
                    entity.LogicalName,
                    entity.Id);

                // TODO: Add business logic for handling Delete message

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteExecutionContextHandler");
                throw;
            }
        }
    }
}
