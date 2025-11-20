using Microsoft.Xrm.Sdk;
using Moq;

namespace Pg.DataverseSync.Domain.Tests.Core
{
    public abstract class ServiceTestBase
    {
        protected readonly ITracingService tracingService;

        public ServiceTestBase()
        {
            var tracingServiceMock = new Mock<ITracingService>();
            tracingServiceMock.Setup(t => t.Trace(It.IsAny<string>()));
            tracingService = tracingServiceMock.Object;
        }
    }
}

