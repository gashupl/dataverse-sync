using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Application.ExecutionContext;
using Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers;
using Pg.DataverseSync.Engine.Application.Source;
using Pg.DataverseSync.Engine.Source;
using Pg.DataverseSync.Engine.Target;
using Pg.DataverseSync.Engine.Target.SqlServer;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Functions;

/// <summary>
/// Program entry point for the Azure Functions application.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        // Register Dataverse service
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

        builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
        builder.Services.AddScoped<ISyncMetadataService, SyncMetadataService>();

        // Register execution context handlers
        builder.Services.AddScoped<IExecutionContextHandler, CreateExecutionContextHandler>();
        builder.Services.AddScoped<IExecutionContextHandler, UpdateExecutionContextHandler>();
        builder.Services.AddScoped<IExecutionContextHandler, DeleteExecutionContextHandler>();

        // Register execution context router
        builder.Services.AddScoped<IExecutionContextRouter, ExecutionContextRouter>();

        //TODO: Reference to target data structure service should be injected based on configuration
        //(e.g. SQL Server, Synapse, etc.) in the future
        builder.Services.AddScoped<IDatabaseSchemaRepository>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["SqlServerConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("SqlServerConnectionString is not configured.");
            }

            var logger = sp.GetRequiredService<ILogger<DatabaseSchemaRepository>>();

            return new DatabaseSchemaRepository(connectionString, logger);
        });
        builder.Services.AddScoped<ITargetDataStructureService, TargetDataStructureService>();

        builder.Build().Run();
    }
}