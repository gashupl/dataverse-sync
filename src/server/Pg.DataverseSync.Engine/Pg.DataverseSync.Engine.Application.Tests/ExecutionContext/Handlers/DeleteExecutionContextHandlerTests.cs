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
    public class DeleteExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DeleteExecutionContextHandler(targetDataRepository, targetRecordFactory, null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new DeleteExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new DeleteExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Delete,
                CorrelationId = Guid.NewGuid(),
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Delete} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CreatesAndDeletesTargetRecord()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new DeleteExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entityReference = new EntityReference("contact", Guid.NewGuid());
            var targetRecord = new TargetRecord("contact");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Delete,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entityReference);
            targetRecordFactory.CreateForDelete(entityReference).Returns(targetRecord);
            targetDataRepository.DeleteRecord(targetRecord).Returns(new TargetRecordModificationResult { Success = true });

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(context));

            // Assert
            Assert.Null(exception);
            targetRecordFactory.Received(1).CreateForDelete(entityReference);
            targetDataRepository.Received(1).DeleteRecord(targetRecord);
        }

        [Fact]
        public async Task HandleAsync_DeleteRecordFails_ThrowsExecutionContextHandlerException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DeleteExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new DeleteExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entityReference = new EntityReference("contact", Guid.NewGuid());
            var targetRecord = new TargetRecord("contact");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Delete,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entityReference);
            var errorMessage = "Record not found in target database";
            targetRecordFactory.CreateForDelete(entityReference).Returns(targetRecord);
            targetDataRepository.DeleteRecord(targetRecord).Returns(new TargetRecordModificationResult 
            { 
                Success = false, 
                Message = errorMessage 
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ExecutionContextHandlerException>(() => handler.HandleAsync(context));
            Assert.Equal(MessageNames.Delete, exception.MessageName);
            Assert.Equal(entityReference.Id, exception.Id);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Contains($"Failed to delete record for entity '{entityReference.LogicalName}' with id '{entityReference.Id}'", exception.InnerException!.Message);
            Assert.Contains(errorMessage, exception.InnerException!.Message);
        }
    }
}
