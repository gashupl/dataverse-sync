using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application
{
    public class TargetSchemaService : LoggingServiceBase<TargetSchemaService>, ITargetSchemaService
    {
        private readonly ITargetSchemaRepository _schemaRepository;

        public TargetSchemaService(ITargetSchemaRepository schemaRepository, ILogger<TargetSchemaService> logger) : base(logger)
        {
            _schemaRepository = schemaRepository;
        }

        public bool TargetTableExists(string tableName)
        {
            return _schemaRepository.TableExists(tableName);  
        }

        public void CreateTargetTable(Table table)
        {
            _schemaRepository.CreateTable(table); 
        }

        public bool IsTargetTableSchemaUpToDate(Table table)
        {
            throw new NotImplementedException();
        }

        public void UpdateTargetTable(Table table)
        {
            throw new NotImplementedException();
        }
    }
}
