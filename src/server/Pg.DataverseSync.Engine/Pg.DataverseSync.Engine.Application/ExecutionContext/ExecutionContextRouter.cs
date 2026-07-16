using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext
{
    /// <summary>
    /// Routes execution context messages to appropriate handlers based on message name.
    /// </summary>
    public class ExecutionContextRouter : IExecutionContextRouter
    {
        private readonly IReadOnlyDictionary<string, IExecutionContextHandler> _handlers;
        private readonly ILogger<ExecutionContextRouter> _logger;

        public ExecutionContextRouter(
            IEnumerable<IExecutionContextHandler> handlers,
            ILogger<ExecutionContextRouter> logger)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            _handlers = handlers.ToDictionary(h => h.MessageName, StringComparer.OrdinalIgnoreCase);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation(
                "ExecutionContextRouter initialized with {handlerCount} handler(s): {handlerNames}",
                _handlers.Count,
                string.Join(", ", _handlers.Keys));
        }

        public async Task RouteAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var messageName = context.MessageName;

            _logger.LogInformation(
                "Routing execution context message: {messageName} (CorrelationId: {correlationId})",
                messageName,
                context.CorrelationId);

            if (!_handlers.TryGetValue(messageName, out var handler))
            {
                _logger.LogError(
                    "No handler found for execution context message type: {messageName}",
                    messageName);

                throw new UnsupportedExecutionContextException(messageName);
            }

            try
            {
                await handler.HandleAsync(context, cancellationToken);
                _logger.LogInformation(
                    "Successfully handled execution context message: {messageName} (CorrelationId: {correlationId})",
                    messageName,
                    context.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling execution context message: {messageName} (CorrelationId: {correlationId})",
                    messageName,
                    context.CorrelationId);
                throw;
            }
        }
    }
}
