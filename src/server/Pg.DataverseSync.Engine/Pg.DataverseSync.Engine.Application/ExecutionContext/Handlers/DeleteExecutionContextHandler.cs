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
    /// Handles 'Delete' execution context messages.
    /// </summary>
    public class DeleteExecutionContextHandler : ServiceBase<DeleteExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Delete;

        public DeleteExecutionContextHandler(ILogger<DeleteExecutionContextHandler> logger) : base(logger)
        {
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

                // TODO: Add business logic for handling Delete message

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
