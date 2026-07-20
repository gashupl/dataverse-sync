using Microsoft.Xrm.Sdk;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext
{
    public interface IExecutionContextHandler
    {
        string MessageName { get; }

        Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default);
    }
}
