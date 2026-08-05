using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Target;

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

        public TargetSchemaModificationResult UpsertTargetTable(Table table)
        {
            try
            {
                if (_schemaRepository.TableExists(table.Name))
                {
                    return _schemaRepository.UpdateTable(table);
                }
                else
                {
                    return _schemaRepository.CreateTable(table);
                }
            }
            catch (Exception ex)
            {
                LogIfEnabled(LogLevel.Error, ex, "An error occurred while upserting table {TableName}.", table.Name);
                return new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResultEnum.Failure,
                    Message = ex.Message
                };
            }
        }

    }
}
