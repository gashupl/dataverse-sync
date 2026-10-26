namespace Pg.DataverseSync.Engine.Application
{
    public class SyncDetailsModificationResult
    {
        public ModificationResult Success { get; set; }

        public string? Message { get; set; }
    }

    public enum ModificationResult
    {
        Success = 0,
        Failure = 1
    }
}
