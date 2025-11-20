using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Plugins.Core;
using System;

namespace Pg.DataverseSync.Plugins.CustomApi
{
    public class GetUnsynchronizedTablesLoader : DependencyLoaderBase
    {
        public GetUnsynchronizedTablesLoader()
        {
            Register<ITablesService, TablesService>();
            Register<IParseToJsonService, ParseToJsonService>();
        }
    }

    public class GetUnsynchronizedTablesPlugin : PluginBase<GetUnsynchronizedTablestHandler>
    {
        public override IDependencyLoader DependencyLoader => new GetUnsynchronizedTablesLoader();

        public GetUnsynchronizedTablesPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(GetUnsynchronizedTablesPlugin))
        {
        }
    }

    public class GetUnsynchronizedTablestHandler : PluginHandlerBase
    {
        private readonly IParseToJsonService _parseToJsonService;
        private readonly ITablesService _tablesService;

        public GetUnsynchronizedTablestHandler(ITablesService tablesService, IParseToJsonService parseToJsonService)
        {
            _tablesService = tablesService;
            _parseToJsonService = parseToJsonService;

        }
        public override bool CanExecute() => true; 

        public override void Execute()
        {
            if (localPluginContext == null)
            {
                throw new InvalidOperationException(nameof(localPluginContext));
            }

            localPluginContext.TracingService.Trace($"Retrieving unsynchronized tables...");

            var tables = _tablesService.GetUnsynchronizedTables(); 

            localPluginContext.TracingService.Trace($"Parsing {tables?.Count} unsynchronized tables...");
            var parsedUnsynchronizedTables = _parseToJsonService.Parse(tables);
            localPluginContext.PluginExecutionContext.OutputParameters["results"] = parsedUnsynchronizedTables;
        }
    }
}

