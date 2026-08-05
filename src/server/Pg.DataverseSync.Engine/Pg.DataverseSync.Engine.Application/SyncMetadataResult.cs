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

        public TableSyncResult(string tableName, bool isSynchronized, string? errorMessage = null)
        {
            TableName = tableName;
            IsSynchronized = isSynchronized;
            ErrorMessage = errorMessage!;
        }
    }
}
