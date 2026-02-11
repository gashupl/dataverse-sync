namespace Pg.DataverseSync.Engine.Functions.Core.Exceptions
{
    public class ReadMetadataException : Exception
    {
        public ReadMetadataException(string message) : base(message)
        {
        }

        public ReadMetadataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
