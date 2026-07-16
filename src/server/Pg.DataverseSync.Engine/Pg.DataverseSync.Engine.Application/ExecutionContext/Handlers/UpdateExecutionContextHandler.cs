using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    /// <summary>
    /// Handles 'Update' execution context messages.
    /// </summary>
    public class UpdateExecutionContextHandler : IExecutionContextHandler
    {
        public string MessageName => MessageNames.Update;

        private readonly ILogger<UpdateExecutionContextHandler> _logger;

        public UpdateExecutionContextHandler(ILogger<UpdateExecutionContextHandler> logger)
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
                    "Processing Update execution context (CorrelationId: {correlationId})",
                    context.CorrelationId);

                if (!context.InputParameters.TryGetValue(ParameterNames.Target, out var targetEntity))
                {
                    throw new InvalidOperationException($"{MessageNames.Update} message missing '{ParameterNames.Target}' in InputParameters.");
                }

                var entity = (Entity)targetEntity;
                _logger.LogInformation(
                    "Update handler: Entity LogicalName={logicalName}, Id={id}",
                    entity.LogicalName,
                    entity.Id);

                // TODO: Add business logic for handling Update message

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateExecutionContextHandler");
                throw;
            }
        }
    }
}
