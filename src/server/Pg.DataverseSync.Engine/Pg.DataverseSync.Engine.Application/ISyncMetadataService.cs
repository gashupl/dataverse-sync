using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Application
{
    public interface ISyncMetadataService
    {
        SyncMetadataResult Execute(); 
    }
}
