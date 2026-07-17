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
            ArgumentNullException.ThrowIfNull(handlers);

            _handlers = handlers.ToDictionary(h => h.MessageName, StringComparer.OrdinalIgnoreCase);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RouteAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            var messageName = context.MessageName;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    "Routing execution context message: {messageName} (CorrelationId: {correlationId})",
                    messageName,
                    context.CorrelationId);
            }


            if (!_handlers.TryGetValue(messageName, out var handler))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(
                    "No handler found for execution context message type: {MessageName}",
                    messageName);
                }

                throw new UnsupportedExecutionContextException(messageName);
            }

            await handler.HandleAsync(context, cancellationToken);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    "Successfully handled execution context message: {MessageName} (CorrelationId: {CorrelationId})",
                    messageName,
                    context.CorrelationId);
            }
        }
    }
}
