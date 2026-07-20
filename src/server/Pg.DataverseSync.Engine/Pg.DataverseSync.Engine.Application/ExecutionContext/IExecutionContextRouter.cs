using Microsoft.Xrm.Sdk;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext
{
    public interface IExecutionContextRouter
    {
        Task RouteAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default);
    }
}
