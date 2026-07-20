using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers;
using Pg.DataverseSync.Engine.Core.ContextConstraints;

namespace Pg.DataverseSync.Engine.Application.Tests.ExecutionContext.Handlers
{
    public class DeleteExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DeleteExecutionContextHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var handler = new DeleteExecutionContextHandler(logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var handler = new DeleteExecutionContextHandler(logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Delete,
                CorrelationId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Delete} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CompletesSuccessfully()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var handler = new DeleteExecutionContextHandler(logger);
            var entityReference = new EntityReference("contact", Guid.NewGuid());
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Delete,
                CorrelationId = Guid.NewGuid()
            };
            context.InputParameters.Add(ParameterNames.Target, entityReference);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(context));

            // Assert
            Assert.Null(exception);
        }
    }
}
