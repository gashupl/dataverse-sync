using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
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
