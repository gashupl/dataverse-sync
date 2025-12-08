using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Pg.DataverseSync.Domain.Services
{
    public class TablesService : ServiceBase, ITablesService
    {
        public TablesService(IRepository repository, ITracingService tracingService) : base(repository, tracingService)
        {
        }

        public List<Table> GetUnsynchronizedTables()
        {

            var syncTablesNames = repository.GetActiveSynchronizedTables()
                .Select(st => st.pg_name)
                .ToList();

            var allStandardTables = repository.GetStandardTablesFromMetadata();

            var unsynchronizedTables = allStandardTables
                .Where(t => !syncTablesNames.Contains(t.SchemaName))
                .ToList();

            return unsynchronizedTables; 

            //return new List<Table>
            //{
            //    new Table { Name = "Account", SchemaName = "account" },
            //    new Table { Name = "Contact", SchemaName = "contact" },
            //    new Table { Name = "Lead", SchemaName = "lead" }
            //};
        }
    }
}

