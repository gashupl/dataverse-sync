using Microsoft.Xrm.Sdk;
using System.Threading;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext
{
    public interface IExecutionContextHandler
    {
        string MessageName { get; }

        Task HandleAsync(RemoteExecutionContext context, CancellationToken cancellationToken = default);
    }
}
