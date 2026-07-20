using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.ExecutionContext;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Tests.ExecutionContext
{
    public class ExecutionContextRouterTests
    {
        [Fact]
        public void Constructor_NullHandlers_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ExecutionContextRouter>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ExecutionContextRouter(null!, logger));
        }

        [Fact]
        public async Task RouteAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ExecutionContextRouter>>();
            var handlers = Enumerable.Empty<IExecutionContextHandler>();
            var router = new ExecutionContextRouter(handlers, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => router.RouteAsync(null!));
        }

        [Fact]
        public async Task RouteAsync_KnownMessageName_InvokesMatchingHandler()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ExecutionContextRouter>>();
            var createHandler = Substitute.For<IExecutionContextHandler>();
            var updateHandler = Substitute.For<IExecutionContextHandler>();
            createHandler.MessageName.Returns(MessageNames.Create);
            updateHandler.MessageName.Returns(MessageNames.Update);

            var router = new ExecutionContextRouter(new[] { createHandler, updateHandler }, logger);
            var context = new RemoteExecutionContext { MessageName = MessageNames.Create.ToLowerInvariant(), CorrelationId = Guid.NewGuid() };

            // Act
            await router.RouteAsync(context);

            // Assert
            await createHandler.Received(1).HandleAsync(context, Arg.Any<CancellationToken>());
            await updateHandler.DidNotReceive().HandleAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RouteAsync_UnknownMessageName_ThrowsUnsupportedExecutionContextException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ExecutionContextRouter>>();
            var createHandler = Substitute.For<IExecutionContextHandler>();
            createHandler.MessageName.Returns(MessageNames.Create);

            var router = new ExecutionContextRouter(new[] { createHandler }, logger);
            var context = new RemoteExecutionContext { MessageName = MessageNames.Merge, CorrelationId = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnsupportedExecutionContextException>(() => router.RouteAsync(context));
            Assert.Equal(MessageNames.Merge, exception.MessageName);
            await createHandler.DidNotReceive().HandleAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RouteAsync_HandlerThrows_RethrowsOriginalException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ExecutionContextRouter>>();
            var createHandler = Substitute.For<IExecutionContextHandler>();
            createHandler.MessageName.Returns(MessageNames.Create);
            createHandler.HandleAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>())
                .Returns(_ => throw new InvalidOperationException("Handler failure"));

            var router = new ExecutionContextRouter(new[] { createHandler }, logger);
            var context = new RemoteExecutionContext { MessageName = MessageNames.Create, CorrelationId = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => router.RouteAsync(context));
            Assert.Equal("Handler failure", exception.Message);
        }
    }
}
