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

            var isValidForCreateUpdate = (context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase)
                || context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                && context.Stage == 40; // PostOperation

            var isValidForDelete = (context.MessageName.Equals("Delete", StringComparison.OrdinalIgnoreCase)
                || context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                && context.Stage == 20; // PreOperation

            // Only execute on PostOperation of Create message
            return (isValidForCreateUpdate || isValidForDelete)
                && context.PrimaryEntityName.Equals(pg_synctable.EntityLogicalName, StringComparison.OrdinalIgnoreCase);
        }

        public override void Execute()
        {
            var context = localPluginContext.PluginExecutionContext;
            var tracingService = localPluginContext.TracingService;
            var messageName = context.MessageName;

            // Handle Delete first — Dataverse sends EntityReference as Target for Delete messages
            if (messageName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                var preImage = context.PreEntityImages["PreImage"];
                var entityName = preImage.GetAttributeValue<string>(pg_synctable.Fields.pg_name);
                tracingService.Trace($"SyncTableServiceBusEndpointStepHandler: Processing Delete for entity '{entityName}'");
                DeleteStepsForEntity(entityName, tracingService);
                return;
            }

            var target = (Entity)context.InputParameters["Target"];
            tracingService.Trace($"SyncTableServiceBusEndpointStepHandler: Processing {messageName} for entity '{target.LogicalName}', Id: {target.Id}");

            if (messageName.Equals("Create", StringComparison.OrdinalIgnoreCase))
            {
                var entityName = target.GetAttributeValue<string>(pg_synctable.Fields.pg_name);
                CreateStepsForEntity(entityName, tracingService);
            }
            else if (messageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
            {
                var preImage = context.PreEntityImages["PreImage"];
                var entityName = preImage.GetAttributeValue<string>(pg_synctable.Fields.pg_name);

                if (IsReactivation(context, target))
                {
                    CreateStepsForEntity(entityName, tracingService);
                }
                else if (IsDeactivation(context, target))
                {
                    DeleteStepsForEntity(entityName, tracingService);
                }
            }
        }

        private void CreateStepsForEntity(string entityName, ITracingService tracingService)
        {
            try
            {
                var result = _endpointStepCreationService.CreateStepsForEntity(entityName, new[] { "Create", "Update", "Delete" });

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

        private void DeleteStepsForEntity(string entityName, ITracingService tracingService)
        {
            try
            {
                var result = _endpointStepCreationService.DeleteStepsForEntity(entityName, new[] { "Create", "Update", "Delete" });
                if (!result.Success)
                {
                    throw new InvalidPluginExecutionException(
                        $"Failed to delete service endpoint steps for entity '{entityName}': {result.ErrorMessage}");
                }
                tracingService.Trace($"Service endpoint steps deleted successfully for entity '{entityName}'.");
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracingService.Trace($"SyncTableServiceBusEndpointStepHandler Error: {ex.Message}");
                throw new InvalidPluginExecutionException(
                    $"Failed to delete service endpoint steps for entity '{entityName}': {ex.Message}", ex);
            }
        }
    }
}
