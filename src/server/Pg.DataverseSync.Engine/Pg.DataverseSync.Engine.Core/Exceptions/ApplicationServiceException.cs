using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ApplicationServiceException : Exception
    {
        public ApplicationServiceException(string message) : base(message)
        {
        }

        public ApplicationServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
