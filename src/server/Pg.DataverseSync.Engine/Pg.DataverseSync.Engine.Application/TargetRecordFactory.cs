using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application
{
    public class TargetRecordFactory : ITargetRecordFactory
    {
        public TargetRecord CreateForInsert(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var tableName = entity.LogicalName;
            var record = new TargetRecord(tableName);

            foreach (var attribute in entity.Attributes)
            {
                record.AddColumn(attribute.Key, attribute.Value);
            }

            return record;
        }

        public TargetRecord CreateForUpdate(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var tableName = entity.LogicalName;
            var record = new TargetRecord(tableName);

            foreach (var attribute in entity.Attributes)
            {
                bool isPrimaryKey = attribute.Key.Equals($"{tableName}id", StringComparison.OrdinalIgnoreCase); 
                record.AddColumn(attribute.Key, attribute.Value, isPrimaryKey);
            }

            return record;
        }

        public TargetRecord CreateForDelete(EntityReference entityReference)
        {
            ArgumentNullException.ThrowIfNull(entityReference);

            var tableName = entityReference.LogicalName;
            var record = new TargetRecord(tableName);

            record.AddColumn($"{tableName}id", entityReference.Id, isPrimaryKey: true);

            return record;
        }

    }
}
