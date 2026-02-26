namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public class SchemaModificationResult
    {
        public SchemaModificationResultEnum Success { get; set; }
        public string? Message { get; set; }
    }

    public enum SchemaModificationResultEnum
    {
        Success = 0,
        PartialSuccess = 1,
        Failure = 2
    }
}
