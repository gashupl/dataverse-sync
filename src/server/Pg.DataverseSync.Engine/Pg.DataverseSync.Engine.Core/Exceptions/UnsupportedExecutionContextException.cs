using System;

namespace Pg.DataverseSync.Engine.Core.Exceptions
{
    public class UnsupportedExecutionContextException : Exception
    {
        public string MessageName { get; }

        public UnsupportedExecutionContextException(string messageName)
            : base($"No handler registered for execution context message type '{messageName}'.")
        {
            MessageName = messageName;
        }

        public UnsupportedExecutionContextException(string messageName, Exception? innerException)
            : base($"No handler registered for execution context message type '{messageName}'.", innerException)
        {
            MessageName = messageName;
        }
    }
}
