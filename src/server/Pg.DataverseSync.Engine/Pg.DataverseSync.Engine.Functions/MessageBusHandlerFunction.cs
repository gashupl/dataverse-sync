using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;
using System;
using System.Text;
using System.Threading.Tasks;

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

        var executionContext = JsonFormatService.DeserializeJsonString<RemoteExecutionContext>(message.Body.ToString());
        var entity = (Entity)executionContext.InputParameters["Target"];
        var start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var name =
            $"{entity.GetAttributeValue<string>("firstname")} {entity.GetAttributeValue<string>("lastname")}";

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }


}