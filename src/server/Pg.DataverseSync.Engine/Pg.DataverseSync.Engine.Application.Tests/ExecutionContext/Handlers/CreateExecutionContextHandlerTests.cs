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
    public class CreateExecutionContextHandlerTests
    {
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CreateExecutionContextHandler(targetDataRepository, targetRecordFactory, null!));
        }

        [Fact]
        public async Task HandleAsync_NullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new CreateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_MissingTarget_ThrowsInvalidOperationException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new CreateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Create,
                CorrelationId = Guid.NewGuid(),
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
            Assert.Equal($"{MessageNames.Create} message missing '{ParameterNames.Target}' in InputParameters.", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ValidTarget_CreatesAndInsertsTargetRecord()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new CreateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            var targetRecord = new TargetRecord("contact");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Create,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entity);
            targetRecordFactory.CreateForInsert(entity).Returns(targetRecord);
            targetDataRepository.InsertRecord(targetRecord).Returns(new TargetRecordModificationResult { Success = true });

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(context));

            // Assert
            Assert.Null(exception);
            targetRecordFactory.Received(1).CreateForInsert(entity);
            targetDataRepository.Received(1).InsertRecord(targetRecord);
        }

        [Fact]
        public async Task HandleAsync_InsertRecordFails_ThrowsExecutionContextHandlerException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<CreateExecutionContextHandler>>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var handler = new CreateExecutionContextHandler(targetDataRepository, targetRecordFactory, logger);
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            var targetRecord = new TargetRecord("contact");
            var context = new RemoteExecutionContext
            {
                MessageName = MessageNames.Create,
                CorrelationId = Guid.NewGuid(),
            };
            context.InputParameters.Add(ParameterNames.Target, entity);
            var errorMessage = "Database connection failed";
            targetRecordFactory.CreateForInsert(entity).Returns(targetRecord);
            targetDataRepository.InsertRecord(targetRecord).Returns(new TargetRecordModificationResult 
            { 
                Success = false, 
                Message = errorMessage 
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ExecutionContextHandlerException>(() => handler.HandleAsync(context));
            Assert.Equal(MessageNames.Create, exception.MessageName);
            Assert.Equal(entity.Id, exception.Id);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Contains($"Failed to Create record for entity '{entity.LogicalName}' with id '{entity.Id}'", exception.InnerException!.Message);
            Assert.Contains(errorMessage, exception.InnerException!.Message);
        }
    }
}
