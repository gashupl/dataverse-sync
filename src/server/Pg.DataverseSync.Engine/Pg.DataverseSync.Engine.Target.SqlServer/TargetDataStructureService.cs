using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Domain.Target;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public class TargetDataStructureService : ITargetDataStructureService
    {
        private readonly IDatabaseSchemaRepository _databaseSchemaRepository;
        private readonly ILogger<TargetDataStructureService> _logger;

        public TargetDataStructureService(
            IDatabaseSchemaRepository databaseSchemaRepository, ILogger<TargetDataStructureService> logger)
        {
            _databaseSchemaRepository = databaseSchemaRepository;
            _logger = logger;
        }

        public UpsertTableResult UpsertTable(Table table)
        {
            throw new NotImplementedException();
        }
    }
}
