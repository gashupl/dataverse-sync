using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Target
{
    public class TargetSchemaModificationResult
    {
        public SchemaModificationResult Success { get; set; }

        public TableSyncOperationCode OperationCode { get; set; }
        public string? Message { get; set; }
    }

    public enum SchemaModificationResult
    {
        Success = 0,
        PartialSuccess = 1,
        Failure = 2
    }
}
