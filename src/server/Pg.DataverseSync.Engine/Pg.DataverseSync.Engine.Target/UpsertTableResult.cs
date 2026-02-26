namespace Pg.DataverseSync.Engine.Target
{
    public class UpsertTableResult
    {
        public UpsertTableResultEnum Result { get; set; }
        public string? Message { get; set; } 

        public UpsertTableResult(UpsertTableResultEnum result, string? message = null)
        {
            Result = result;
            Message = message;
        }
    }

    public enum UpsertTableResultEnum
    {
        TableCreated = 0,
        TableUpdated = 1,
        TableUnchanged = 2,
        Error = 3
    }
}
