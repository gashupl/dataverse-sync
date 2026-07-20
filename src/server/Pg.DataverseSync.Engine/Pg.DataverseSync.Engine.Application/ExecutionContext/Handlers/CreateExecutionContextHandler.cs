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
    public class CreateExecutionContextHandler : ServiceBase<CreateExecutionContextHandler>, IExecutionContextHandler
    {
        public string MessageName => MessageNames.Create;

        public CreateExecutionContextHandler(ILogger<CreateExecutionContextHandler> logger) : base(logger)
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

                // TODO: Add business logic for handling Create message

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
