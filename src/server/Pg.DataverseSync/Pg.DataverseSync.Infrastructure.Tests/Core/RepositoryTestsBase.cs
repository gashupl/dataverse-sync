using Microsoft.Xrm.Sdk;
using Moq;

namespace Pg.DataverseSync.Infrastructure.Tests.Core
{
    public abstract class RepositoryTestsBase
    {
        protected ITracingService tracingService { get; }

        protected RepositoryTestsBase()
        {
            tracingService = new Mock<ITracingService>().Object;
        }
    }
}
