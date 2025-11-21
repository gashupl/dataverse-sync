using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using System;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Services
{
    public class TablesService : ServiceBase, ITablesService
    {
        public TablesService(IRepository repository, ITracingService tracingService) : base(repository, tracingService)
        {
        }

        public List<Table> GetUnsynchronizedTables()
        {
            return new List<Table>
            {
                new Table { Name = "Account", SchemaName = "account" },
                new Table { Name = "Contact", SchemaName = "contact" },
                new Table { Name = "Lead", SchemaName = "lead" }
            };
        }
    }
}

