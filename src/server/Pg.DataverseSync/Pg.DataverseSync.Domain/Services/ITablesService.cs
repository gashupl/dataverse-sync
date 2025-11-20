using Pg.DataverseSync.Domain.Dto;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Services
{
    public interface ITablesService
    {
        List<Table> GetUnsynchronizedTables();
    }
}

