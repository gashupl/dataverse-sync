using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Model;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IRepository
    {
        List<Table> GetStandardTablesFromMetadata();
        List<pg_synctable> GetActiveSynchronizedTables();
    }
}

