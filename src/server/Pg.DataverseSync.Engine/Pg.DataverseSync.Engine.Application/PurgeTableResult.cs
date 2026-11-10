namespace Pg.DataverseSync.Engine.Application
{
    public class PurgeTableResult
    {
        public string TableName { get; set; }
        public bool IsPurged { get; set; }
        public string ErrorMessage { get; set; }

        public PurgeTableResult(string tableName, bool isPurged, string? errorMessage = null)
        {
            TableName = tableName;
            IsPurged = isPurged;
            ErrorMessage = errorMessage!;
        }   
    }
}
