using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Core.Exceptions
{
    public class DomainServiceException : Exception
    {
        public DomainServiceException(string message) : base(message)
        {
        }

        public DomainServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
