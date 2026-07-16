using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Application.ExecutionContext;
using Pg.DataverseSync.Engine.Core.Exceptions;
using System;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Engine.Functions;

public class MessageBusHandlerFunction
{
    private readonly ILogger<MessageBusHandlerFunction> _logger;
    private readonly IExecutionContextRouter _executionContextRouter;

    public MessageBusHandlerFunction(
        ILogger<MessageBusHandlerFunction> logger,
        IExecutionContextRouter executionContextRouter)
    {
        _logger = logger;
        _executionContextRouter = executionContextRouter;
    }

    [Function(nameof(MessageBusHandlerFunction))]
    public async Task Run(
        [ServiceBusTrigger("dv-sync-queue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        try
        {
            var executionContext = JsonFormatService.DeserializeJsonString<RemoteExecutionContext>(message.Body.ToString());
            
            // Route to appropriate handler
            await _executionContextRouter.RouteAsync(executionContext);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
        catch (UnsupportedExecutionContextException ex)
        {
            _logger.LogError(ex, "Unsupported execution context message type: {messageName}", ex.MessageName);
            // Move to dead-letter queue
            var deadLetterOptions = new Dictionary<string, object>
            {
                { "UserProperties", new Dictionary<string, object> { { "Reason", "UnsupportedMessageType" } } }
            };
            await messageActions.DeadLetterMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            // Move to dead-letter queue
            await messageActions.DeadLetterMessageAsync(message);
        }
    }
}