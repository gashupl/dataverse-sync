using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;

namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IRepository
    {
        //TODO [HIGH]: Split single repository into multiple repositories for better separation of concerns
        List<Table> GetStandardTablesFromMetadata();
        List<pg_synctable> GetActiveSynchronizedTables();

        void CreateStep(SdkMessageProcessingStep step);

        bool StepExists(Guid serviceEndpointId, string messageName, string entityName); 


    }
}

