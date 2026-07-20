using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext
{
    /// <summary>
    /// Routes execution context messages to appropriate handlers based on message name.
    /// </summary>
    public class ExecutionContextRouter : ServiceBase<ExecutionContextRouter>, IExecutionContextRouter
    {
        private readonly Dictionary<string, IExecutionContextHandler> _handlers;

        public ExecutionContextRouter(
            IEnumerable<IExecutionContextHandler> handlers,
            ILogger<ExecutionContextRouter> logger) : base(logger) 
        {
            ArgumentNullException.ThrowIfNull(handlers);

            _handlers = handlers.ToDictionary(h => h.MessageName, StringComparer.OrdinalIgnoreCase);
        }

        public async Task RouteAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            var messageName = context.MessageName;

            LogIfEnabled(LogLevel.Information,
                "Routing execution context message: {MessageName} (CorrelationId: {CorrelationId})",
                messageName,
                context.CorrelationId);


            if (!_handlers.TryGetValue(messageName, out var handler))
            {
                LogIfEnabled(LogLevel.Error,
                    "No handler found for execution context message type: {MessageName}",
                    messageName);

                throw new UnsupportedExecutionContextException(messageName);
            }

            await handler.HandleAsync(context, cancellationToken);
            LogIfEnabled(LogLevel.Information,
                "Successfully handled execution context message: {MessageName} (CorrelationId: {CorrelationId})",
                messageName,
                context.CorrelationId);
        }
    }
}
