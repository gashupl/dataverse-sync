namespace Pg.DataverseSync.Engine.Application.Data
{
    public class TargetSchemaModificationResult
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
