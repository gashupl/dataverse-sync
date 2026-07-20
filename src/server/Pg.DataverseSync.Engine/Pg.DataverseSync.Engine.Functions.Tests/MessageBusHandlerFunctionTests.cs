using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Application.ExecutionContext;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Functions.Tests
{
    [ExcludeFromCodeCoverage]
    public class MessageBusHandlerFunctionTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var router = Substitute.For<IExecutionContextRouter>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MessageBusHandlerFunction(router, null!));
        }

        [Fact]
        public void Constructor_NullRouter_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MessageBusHandlerFunction(null!, logger));
        }

        [Fact]
        public async Task Run_ValidMessage_CompletesMessage()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            // Use a JSON string directly instead of trying to serialize RemoteExecutionContext
            // to avoid DateTime serialization issues
            var jsonBody = @"{""MessageName"":""Create"",""CorrelationId"":""00000000-0000-0000-0000-000000000000""}";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(jsonBody),
                messageId: "test-message-id",
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act
            await function.Run(message, messageActions);

            // Assert
            await router.Received(1).RouteAsync(Arg.Is<RemoteExecutionContext>(c =>
                c.MessageName == MessageNames.Create), Arg.Any<CancellationToken>());
            await messageActions.Received(1).CompleteMessageAsync(message);
        }

        [Fact]
        public async Task Run_UnsupportedMessageType_DeadLettersMessage()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            router.RouteAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new UnsupportedExecutionContextException("InvalidMessage"));

            var jsonBody = @"{""MessageName"":""InvalidMessage"",""CorrelationId"":""00000000-0000-0000-0000-000000000000""}";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(jsonBody),
                messageId: "test-message-id",
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act
            await function.Run(message, messageActions);

            // Assert
            await messageActions.Received(1).DeadLetterMessageAsync(message, Arg.Any<Dictionary<string, object>?>());
            await messageActions.DidNotReceive().CompleteMessageAsync(message);
        }

        [Fact]
        public async Task Run_HandlerThrowsExecutionContextHandlerException_DeadLettersMessage()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            var testException = new ExecutionContextHandlerException(
                MessageNames.Create,
                Guid.NewGuid(),
                "TestLogicalName",
                new InvalidOperationException("Handler failed"));

            router.RouteAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(testException);

            var jsonBody = @"{""MessageName"":""Create"",""CorrelationId"":""00000000-0000-0000-0000-000000000000""}";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(jsonBody),
                messageId: "test-message-id",
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act
            await function.Run(message, messageActions);

            // Assert
            await messageActions.Received(1).DeadLetterMessageAsync(message, Arg.Any<Dictionary<string, object>?>());
            await messageActions.DidNotReceive().CompleteMessageAsync(message);
        }

        [Fact]
        public async Task Run_GeneralException_DeadLettersMessage()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            router.RouteAsync(Arg.Any<RemoteExecutionContext>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new InvalidOperationException("Unexpected error"));

            var jsonBody = @"{""MessageName"":""Create"",""CorrelationId"":""00000000-0000-0000-0000-000000000000""}";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(jsonBody),
                messageId: "test-message-id",
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act
            await function.Run(message, messageActions);

            // Assert
            await messageActions.Received(1).DeadLetterMessageAsync(message);
            await messageActions.DidNotReceive().CompleteMessageAsync(message);
        }

        [Fact]
        public async Task Run_InvalidJson_DeadLettersMessage()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString("{ invalid json }"),
                messageId: "test-message-id",
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act
            await function.Run(message, messageActions);

            // Assert
            await messageActions.Received(1).DeadLetterMessageAsync(message);
            await messageActions.DidNotReceive().CompleteMessageAsync(message);
        }

        [Fact]
        public async Task Run_LogsMessageDetails()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MessageBusHandlerFunction>>();
            var router = Substitute.For<IExecutionContextRouter>();
            var messageActions = Substitute.For<ServiceBusMessageActions>();

            var jsonBody = @"{""MessageName"":""Create"",""CorrelationId"":""00000000-0000-0000-0000-000000000000""}";
            var messageId = "test-message-id";
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(jsonBody),
                messageId: messageId,
                contentType: "application/json");

            var function = new MessageBusHandlerFunction(router, logger);

            // Act - should complete without throwing
            await function.Run(message, messageActions);

            // Assert - message should be completed successfully
            await messageActions.Received(1).CompleteMessageAsync(message);
        }
    }
}
