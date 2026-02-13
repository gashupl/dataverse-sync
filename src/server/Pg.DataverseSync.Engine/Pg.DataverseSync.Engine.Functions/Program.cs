using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Domain;
using Pg.DataverseSync.Engine.Domain.Source;
using Pg.DataverseSync.Engine.Source;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register Dataverse service
//TODO: Add Key Vault integration for connection string
builder.Services.AddScoped<IOrganizationService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["DataverseConnectionString"];
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DataverseConnectionString is not configured.");
    }
    
    var serviceClient = new ServiceClient(connectionString);
    
    if (!serviceClient.IsReady)
    {
        throw new InvalidOperationException($"Failed to connect to Dataverse: {serviceClient.LastError}");
    }
    
    return serviceClient;
});

// Register MetadataReader
builder.Services.AddScoped<IMetadataReader, MetadataReader>();
builder.Services.AddScoped<ISourceMetadataService, SourceMetadataService>();

builder.Build().Run();
