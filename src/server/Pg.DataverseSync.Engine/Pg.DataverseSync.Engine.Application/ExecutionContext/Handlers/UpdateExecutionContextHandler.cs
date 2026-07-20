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
    /// Handles 'Update' execution context messages.
    /// </summary>
    public class UpdateExecutionContextHandler : ServiceBase<UpdateExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Update;

        public UpdateExecutionContextHandler(ILogger<UpdateExecutionContextHandler> logger) : base(logger)
        {
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

                // TODO: Add business logic for handling Update message

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
