using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public class TargetDataStructureService : ITargetDataStructureService
    {
        private readonly ITargetSchemaRepository _targetSchemaRepository;
        private readonly ILogger<TargetDataStructureService> _logger;

        public TargetDataStructureService(
            ITargetSchemaRepository targetSchemaRepository, ILogger<TargetDataStructureService> logger)
        {
            _targetSchemaRepository = targetSchemaRepository    ;
            _logger = logger;
        }

        public UpsertTableResult UpsertTable(Table table)
        {
            throw new NotImplementedException();
        }
    }
}
