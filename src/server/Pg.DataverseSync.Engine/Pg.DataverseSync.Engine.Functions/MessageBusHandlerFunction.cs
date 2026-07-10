using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Pg.DataverseSync.Engine.Functions;

public class MessageBusHandlerFunction
{
    private readonly ILogger<MessageBusHandlerFunction> _logger;

    public MessageBusHandlerFunction(ILogger<MessageBusHandlerFunction> logger)
    {
        _logger = logger;
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

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}