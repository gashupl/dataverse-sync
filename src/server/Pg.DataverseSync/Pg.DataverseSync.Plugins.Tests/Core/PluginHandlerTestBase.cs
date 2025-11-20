using Microsoft.Xrm.Sdk;
using Moq;

namespace Pg.DataverseSync.Plugins.Tests.Core
{
    public abstract class PluginHandlerTestBase
    {

        public ILocalPluginContext CreateLocalPluginContext()
        {
            var localPluginContextMock = CreateLocalPluginHandlerMock();    
            localPluginContextMock.Setup(c => c.PluginExecutionContext.InputParameters)
                .Returns(new ParameterCollection());

            return localPluginContextMock.Object;
        }

        public ILocalPluginContext CreateLocalPluginContext(ParameterCollection inputParameters)
        {
            var localPluginContextMock = CreateLocalPluginHandlerMock();
            localPluginContextMock.Setup(c => c.PluginExecutionContext.InputParameters)
                .Returns(inputParameters);

            return localPluginContextMock.Object;
        }

        private Mock<ILocalPluginContext> CreateLocalPluginHandlerMock()
        {
            var tracingServiceMock = new Mock<ITracingService>();
            tracingServiceMock.Setup(ts => ts.Trace(It.IsAny<string>()));

            var localPluginContextMock = new Mock<ILocalPluginContext>();
            localPluginContextMock.Setup(c => c.TracingService)
                .Returns(tracingServiceMock.Object);

            localPluginContextMock.Setup(c => c.PluginExecutionContext.OutputParameters)
    .           Returns(new ParameterCollection());

            return localPluginContextMock;
        }
    }
}

