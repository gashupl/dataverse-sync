using Pg.DataverseSync.Model;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface ISyncTablesRepository : IRepository
    {
        List<pg_synctable> GetActiveSynchronizedTables();
    }
}
