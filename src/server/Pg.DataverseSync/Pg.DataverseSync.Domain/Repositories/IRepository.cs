using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IRepository
    {
        List<Table> GetStandardTablesFromMetadata();
        
        Guid GetSdkMessageId(string messageName);

        Guid? GetSdkMessageFilterId(string messageName, string entityName);


    }
}

