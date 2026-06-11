using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface ISyncTablesRepository : IRepository
    {
        List<pg_synctable> GetActiveSynchronizedTables();
    }
}
