using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers;
using Pg.DataverseSync.Engine.Core.ContextConstraints;

namespace Pg.DataverseSync.Engine.Application.Tests.ExecutionContext.Handlers
{
    public class UpdateExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UpdateExecutionContextHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var handler = new UpdateExecutionContextHandler(logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var handler = new UpdateExecutionContextHandler(logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Update,
                CorrelationId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Update} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CompletesSuccessfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var handler = new UpdateExecutionContextHandler(logger);
            var entity = new Entity("account") { Id = Guid.NewGuid() };
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Update,
                CorrelationId = Guid.NewGuid()
            };
            context.InputParameters.Add(ParameterNames.Target, entity);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(context));

            // Assert
            Assert.Null(exception);
        }
    }
}
