using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Domain;

namespace Pg.DataverseSync.Engine.Functions;

public class SyncEngineFunction
{
    private readonly ISourceMetadataService _metadataService;
    private readonly ILogger<SyncEngineFunction> _logger;

    public SyncEngineFunction(ISourceMetadataService metadataService, ILogger<SyncEngineFunction> logger)
    {
        _metadataService = metadataService;
        _logger = logger;
    }

    [Function(nameof(SyncEngineFunction))]
    public async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(SyncEngineFunction));
        logger.LogInformation("Saying hello.");
        var outputs = new List<string>();

        outputs.Add(await context.CallActivityAsync<string>(nameof(ReadMetadata), "ReadMetadata"));

        // Replace name and input with values relevant for your Durable Functions Activity
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [Function(nameof(SayHello))]
    public string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

    [Function(nameof(ReadMetadata))]
    public string ReadMetadata([ActivityTrigger] string input, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(ReadMetadata));
        logger.LogInformation("Reading metadata from Dataverse...");

        var tables = _metadataService.GetTablesNames(); 
        
        if (tables == null)
        {
            logger.LogError("Failed to retrieve tables from Dataverse.");
            return "Failed to retrieve metadata.";
        }
        
        logger.LogInformation("Retrieved {TableCount} tables names from Dataverse.", tables.Count);
        return $"Retrieved {tables.Count} tables from Dataverse.";
    }

    [Function("Function1_HttpStart")]
    public async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Function1_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(SyncEngineFunction));

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}