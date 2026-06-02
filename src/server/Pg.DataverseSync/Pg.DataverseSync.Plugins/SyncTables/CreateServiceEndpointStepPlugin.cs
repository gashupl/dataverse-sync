using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Model;
using Pg.DataverseSync.Plugins.Core;
using System;

namespace Pg.DataverseSync.Plugins.SyncTables
{
    public class CreateServiceEndpointStepLoader : DependencyLoaderBase
    {
        public CreateServiceEndpointStepLoader()
        {
            Register<IEndpointStepCreationService, EndpointStepCreationService>();
        }
    }

    public class CreateServiceEndpointStepPlugin : PluginBase<CreateServiceEndpointStepHandler>
    {
        public override IDependencyLoader DependencyLoader => new CreateServiceEndpointStepLoader();

        public CreateServiceEndpointStepPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(CreateServiceEndpointStepPlugin))
        {
        }
    }

    public class CreateServiceEndpointStepHandler : PluginHandlerBase
    {
        private readonly IEndpointStepCreationService _endpointStepCreationService;

        public CreateServiceEndpointStepHandler(IEndpointStepCreationService endpointStepCreationService)
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

            tracingService.Trace($"CreateServiceEndpointStepHandler: Processing {context.MessageName} for entity '{target.LogicalName}', Id: {target.Id}");

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
                var result = _endpointStepCreationService.CreateStepForEntity(entityName);

                if (!result.Success)
                {
                    throw new InvalidPluginExecutionException(
                        $"Failed to create service endpoint step for entity '{entityName}': {result.ErrorMessage}");
                }

                tracingService.Trace($"Service endpoint step created successfully for entity '{entityName}'.");
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracingService.Trace($"CreateServiceEndpointStepHandler Error: {ex.Message}");
                throw new InvalidPluginExecutionException(
                    $"Failed to create service endpoint step for entity '{entityName}': {ex.Message}", ex);
            }
        }
    }
}
