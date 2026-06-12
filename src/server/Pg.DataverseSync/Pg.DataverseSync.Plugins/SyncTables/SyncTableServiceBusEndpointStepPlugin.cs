using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Infrastructure.Repositories;
using Pg.DataverseSync.Model;
using Pg.DataverseSync.Plugins.Core;
using System;

namespace Pg.DataverseSync.Plugins.SyncTables
{
    public class SyncTableServiceBusEndpointStepLoader : DependencyLoaderBase
    {
        public SyncTableServiceBusEndpointStepLoader()
        {
            Register<IServiceBusEndpointsRepository, ServiceBusEndpointsRepository>(); 
            Register<IEnvironmentVariablesRepository, EnvironmentVariablesRepository>(); 
            Register<IServiceBusStepService, ServiceBusStepService>();
        }
    }

    public class SyncTableServiceBusEndpointStepPlugin : PluginBase<SyncTableServiceBusEndpointStepHandler>
    {
        public override IDependencyLoader DependencyLoader => new SyncTableServiceBusEndpointStepLoader();

        public SyncTableServiceBusEndpointStepPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(SyncTableServiceBusEndpointStepPlugin))
        {
        }
    }

    public class SyncTableServiceBusEndpointStepHandler : PluginHandlerBase
    {
        private readonly IServiceBusStepService _endpointStepCreationService;

        public SyncTableServiceBusEndpointStepHandler(IServiceBusStepService endpointStepCreationService)
        {
            _endpointStepCreationService = endpointStepCreationService;
        }

        public override bool CanExecute()
        {
            var context = localPluginContext.PluginExecutionContext;

            // Only execute on PostOperation of Create message
            return (context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase) 
                || context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                && context.Stage == 40 // PostOperation
                && context.PrimaryEntityName.Equals(pg_synctable.EntityLogicalName, StringComparison.OrdinalIgnoreCase);
        }

        public override void Execute()
        {
            var context = localPluginContext.PluginExecutionContext;
            var tracingService = localPluginContext.TracingService;
            var target = (Entity)context.InputParameters["Target"];

            tracingService.Trace($"SyncTableServiceBusEndpointStepHandler: Processing {context.MessageName} for entity '{target.LogicalName}', Id: {target.Id}");

            var entityName = target.GetAttributeValue<string>(pg_synctable.Fields.pg_name);

            if (context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
            {
                if (!IsReactivation(context, target))
                {
                    tracingService.Trace("Update detected but not a reactivation. Skipping.");
                    return;
                }

                if (string.IsNullOrEmpty(entityName))
                {
                    var preImage = context.PreEntityImages["PreImage"];
                    entityName = preImage.GetAttributeValue<string>(pg_synctable.Fields.pg_name);
                }
            }

            try
            {
                //TODO: It should be replaced with the Create, Update and Delete calls. Message name should not be hardcoded. 
                var result = _endpointStepCreationService.CreateStepsForEntity(entityName, new[] { "Create" });

                if (!result.Success)
                {
                    throw new InvalidPluginExecutionException(
                        $"Failed to create service endpoint steps for entity '{entityName}': {result.ErrorMessage}");
                }

                tracingService.Trace($"Service endpoint steps created successfully for entity '{entityName}'.");
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracingService.Trace($"SyncTableServiceBusEndpointStepHandler Error: {ex.Message}");
                throw new InvalidPluginExecutionException(
                    $"Failed to create service endpoint steps for entity '{entityName}': {ex.Message}", ex);
            }
        }
    }
}
