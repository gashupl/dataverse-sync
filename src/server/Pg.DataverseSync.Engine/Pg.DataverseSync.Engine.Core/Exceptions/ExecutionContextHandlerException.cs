using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Core.Exceptions
{
    public class ExecutionContextHandlerException : Exception
    {
        public string MessageName { get; }
        public Guid Id { get; }
        public string LogicalName { get; }

        public ExecutionContextHandlerException(string messageName, Guid id, string logicalName)
            : base($"Error handling execution context message '{messageName}' (Id: {id}, LogicalName: {logicalName})")
        {
            MessageName = messageName;
            Id = id;
            LogicalName = logicalName;
        }

        public ExecutionContextHandlerException(string messageName, Guid id, string logicalName, Exception? innerException)
            : base($"Error handling execution context message '{messageName}' (Id: {id}, LogicalName: {logicalName})", innerException)
        {
            MessageName = messageName;
            Id = id;
            LogicalName = logicalName;
        }
    }
}
