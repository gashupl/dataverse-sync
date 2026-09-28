using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers;
using Pg.DataverseSync.Engine.Core.ContextConstraints;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Tests.ExecutionContext.Handlers
{
    public class UpdateExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UpdateExecutionContextHandler(targetDataRepository, targetRecordFactory, null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new UpdateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new UpdateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Update,
                CorrelationId = Guid.NewGuid(),
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Update} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CreatesAndUpdatesTargetRecord()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new UpdateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entity = new Entity("account") { Id = Guid.NewGuid() };
            var targetRecord = new TargetRecord("account");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Update,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entity);
            targetRecordFactory.CreateForUpdate(entity).Returns(targetRecord);
            targetDataRepository.UpdateRecord(targetRecord).Returns(new TargetRecordModificationResult { Success = true });

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(context));

            // Assert
            Assert.Null(exception);
            targetRecordFactory.Received(1).CreateForUpdate(entity);
            targetDataRepository.Received(1).UpdateRecord(targetRecord);
        }

        [Fact]
        public async Task HandleAsync_UpdateRecordFails_ThrowsExecutionContextHandlerException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<UpdateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new UpdateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entity = new Entity("account") { Id = Guid.NewGuid() };
            var targetRecord = new TargetRecord("account");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Update,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entity);
            var errorMessage = "Primary key column not found";
            targetRecordFactory.CreateForUpdate(entity).Returns(targetRecord);
            targetDataRepository.UpdateRecord(targetRecord).Returns(new TargetRecordModificationResult 
            { 
                Success = false, 
                Message = errorMessage 
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ExecutionContextHandlerException>(() => handler.HandleAsync(context));
            Assert.Equal(MessageNames.Update, exception.MessageName);
            Assert.Equal(entity.Id, exception.Id);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Contains($"Failed to Update record for entity '{entity.LogicalName}' with id '{entity.Id}'", exception.InnerException!.Message);
            Assert.Contains(errorMessage, exception.InnerException!.Message);
        }
    }
}
