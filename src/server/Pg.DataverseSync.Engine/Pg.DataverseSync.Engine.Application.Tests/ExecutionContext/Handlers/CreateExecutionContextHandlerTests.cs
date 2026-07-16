using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers;
using Pg.DataverseSync.Engine.Core.ContextConstraints;

namespace Pg.DataverseSync.Engine.Application.Tests.ExecutionContext.Handlers
{
    public class CreateExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CreateExecutionContextHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var handler = new CreateExecutionContextHandler(logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var handler = new CreateExecutionContextHandler(logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Create,
                CorrelationId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Create} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CompletesSuccessfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var handler = new CreateExecutionContextHandler(logger);
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Create,
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
