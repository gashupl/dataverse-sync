using Pg.DataverseSync.Model;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IRepository
    {
        List<Table> GetTablesFromMetadata();
        List<pg_synctable> GetActiveSynchronizedTables();
    }
}

