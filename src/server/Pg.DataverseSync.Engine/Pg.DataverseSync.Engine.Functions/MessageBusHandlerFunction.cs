using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Application.ExecutionContext;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Functions;

public class MessageBusHandlerFunction : LoggingServiceBase<MessageBusHandlerFunction>
{
    private readonly IExecutionContextRouter _executionContextRouter;

    public MessageBusHandlerFunction(
        IExecutionContextRouter executionContextRouter, 
        ILogger<MessageBusHandlerFunction> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(executionContextRouter);

        _executionContextRouter = executionContextRouter;
    }


    [Function(nameof(MessageBusHandlerFunction))]
    public async Task Run(
        [ServiceBusTrigger("dv-sync-queue", Connection = "ServiceBusConnectionString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
    {
        LogIfEnabled(LogLevel.Information, "Message ID: {Id}; Body: {Body}; {ContentType}", 
            message.MessageId, 
            message.Body, 
            message.ContentType);


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
            LogIfEnabled(LogLevel.Error, ex, "Unsupported execution context message type: {MessageName}", ex.MessageName);
            // Move to dead-letter queue
            var deadLetterOptions = new Dictionary<string, object>
            {
                { "UserProperties", new Dictionary<string, object> { { "Reason", "UnsupportedMessageType" } } }
            };
            await messageActions.DeadLetterMessageAsync(message, deadLetterOptions);
        }
        catch(ExecutionContextHandlerException ex)
        {
            LogIfEnabled(LogLevel.Error, ex, 
                    "Error processing execution context message: {MessageName}, Id: {Id}, LogicalName: {LogicalName}", 
                    ex.MessageName, 
                    ex.Id, 
                    ex.LogicalName);

            // Move to dead-letter queue
            var deadLetterOptions = new Dictionary<string, object>
            {
                { "UserProperties", new Dictionary<string, object> { { "Reason", "HandlerProcessingError" } } }
            };

            await messageActions.DeadLetterMessageAsync(message, deadLetterOptions);
        }
        catch (Exception ex)
        {
            LogIfEnabled(LogLevel.Error, ex, "Error processing message");
            // Move to dead-letter queue
            await messageActions.DeadLetterMessageAsync(message);
        }
    }
}