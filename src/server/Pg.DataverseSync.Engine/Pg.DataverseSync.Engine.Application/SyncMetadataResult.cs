using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Application
{
    public class SyncMetadataResult
    {
        public List<TableSyncResult> TablesSyncResult { get; set; } = new List<TableSyncResult>(); 
    }

    public class TableSyncResult
    {
        public string TableName { get; set; }
        public bool IsSynchronized { get; set; }
        public string ErrorMessage { get; set; }

        public TableSyncOperationCode OperationCode { get; set; }

        public TableSyncResult(string tableName, bool isSynchronized, TableSyncOperationCode operationCode = TableSyncOperationCode.None, string? errorMessage = null)
        {
            TableName = tableName;
            IsSynchronized = isSynchronized;
            OperationCode = operationCode;
            ErrorMessage = errorMessage!;
        }
    }
}
