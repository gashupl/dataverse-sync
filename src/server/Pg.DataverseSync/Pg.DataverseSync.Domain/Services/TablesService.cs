using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Pg.DataverseSync.Domain.Services
{
    public class TablesService : ServiceBase, ITablesService
    {
        private readonly ISyncTablesRepository _syncTablesRepository;

        public TablesService(ISyncTablesRepository syncTablesRepository, ITracingService tracingService) : base(tracingService)
        {
            _syncTablesRepository = syncTablesRepository;
        }

        public List<Table> GetUnsynchronizedTables()
        {

            var syncTablesNames = _syncTablesRepository.GetActiveSynchronizedTables()
                .Select(st => st.pg_name)
                .ToList();

            var allStandardTables = _syncTablesRepository.GetStandardTablesFromMetadata();

            var unsynchronizedTables = allStandardTables
                .Where(t => !syncTablesNames.Contains(t.SchemaName))
                .ToList();

            return unsynchronizedTables; 
        }
    }
}

