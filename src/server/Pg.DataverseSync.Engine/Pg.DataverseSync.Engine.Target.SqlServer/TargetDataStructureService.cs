using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Domain.Target;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public class TargetDataStructureService : ITargetDataStructureService
    {
        private readonly IDatabaseSchemaRepository _databaseSchemaRepository;

        public TargetDataStructureService(IDatabaseSchemaRepository databaseSchemaRepository)
        {
            _databaseSchemaRepository = databaseSchemaRepository;
        }

        public UpsertTableResult UpsertTable(Table table)
        {
            throw new NotImplementedException();
        }
    }
}
