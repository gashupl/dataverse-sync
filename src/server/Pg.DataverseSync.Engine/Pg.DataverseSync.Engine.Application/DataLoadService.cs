using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Target;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Application
{
    public class DataLoadService : LoggingServiceBase<DataLoadService>, IDataLoadService
    {


        public DataLoadService(ILogger<DataLoadService> logger)
            : base(logger)
        {
        }

        public List<TargetRecordModificationResult> LoadInitialData(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
