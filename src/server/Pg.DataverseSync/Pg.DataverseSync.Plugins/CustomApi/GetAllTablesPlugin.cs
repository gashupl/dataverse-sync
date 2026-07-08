using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Infrastructure.Repositories;
using Pg.DataverseSync.Model;
using Pg.DataverseSync.Plugins.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Plugins.CustomApi
{
    [ExcludeFromCodeCoverage]
    public class GetAllTablesLoader : DependencyLoaderBase
    {
        public GetAllTablesLoader()
        {
            Register<ISyncTablesRepository, SyncTablesRepository>();
            Register<ITablesService, TablesService>();
            Register<IParseToJsonService, ParseToJsonService>();
        }
    }

    [ExcludeFromCodeCoverage]
    public class GetAllTablesPlugin : PluginBase<GetAllTablesHandler>
    {
        public override IDependencyLoader DependencyLoader => new GetAllTablesLoader();

        public GetAllTablesPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(GetAllTablesPlugin))
        {
        }
    }

    public class GetAllTablesHandler : PluginHandlerBase
    {
        private readonly IParseToJsonService _parseToJsonService;
        private readonly ITablesService _tablesService;

        public GetAllTablesHandler(ITablesService tablesService, IParseToJsonService parseToJsonService)
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

            localPluginContext.TracingService.Trace("Retrieving all tables...");

            var tables = _tablesService.GetAllTables();

            localPluginContext.TracingService.Trace($"Parsing {tables?.Count} all tables...");
            var parsedAllTables = _parseToJsonService.Parse(tables);

            localPluginContext.PluginExecutionContext
                .OutputParameters[pg_gettablesResponse.Fields.alltables] = parsedAllTables;
        }
    }
}
