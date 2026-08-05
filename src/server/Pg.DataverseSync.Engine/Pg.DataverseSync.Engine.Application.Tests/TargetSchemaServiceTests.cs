using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class TargetSchemaServiceTests
    {
        [Fact]
        public void TargetTableExists_TableExists_ReturnsTrue()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            const string tableName = "account";

            mockSchemaRepository.TableExists(tableName).Returns(true);

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.TargetTableExists(tableName);

            // Assert
            Assert.True(result);
            mockSchemaRepository.Received(1).TableExists(tableName);
        }

        [Fact]
        public void TargetTableExists_TableDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            const string tableName = "account";

            mockSchemaRepository.TableExists(tableName).Returns(false);

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.TargetTableExists(tableName);

            // Assert
            Assert.False(result);
            mockSchemaRepository.Received(1).TableExists(tableName);
        }

        [Fact]
        public void UpsertTargetTable_TableDoesNotExist_CreatesTableSuccessfully()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            var table = new Table("account", "Account", false);

            mockSchemaRepository.TableExists(table.Name).Returns(false);
            mockSchemaRepository.CreateTable(table)
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResultEnum.Success });

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.UpsertTargetTable(table);

            // Assert
            Assert.Equal(SchemaModificationResultEnum.Success, result.Success);
            mockSchemaRepository.Received(1).TableExists(table.Name);
            mockSchemaRepository.Received(1).CreateTable(table);
            mockSchemaRepository.DidNotReceive().UpdateTable(Arg.Any<Table>());
        }

        [Fact]
        public void UpsertTargetTable_TableDoesNotExist_CreationFails()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            var table = new Table("account", "Account", false);
            var failureMessage = "Database connection failed";

            mockSchemaRepository.TableExists(table.Name).Returns(false);
            mockSchemaRepository.CreateTable(table)
                .Returns(new TargetSchemaModificationResult 
                { 
                    Success = SchemaModificationResultEnum.Failure, 
                    Message = failureMessage 
                });

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.UpsertTargetTable(table);

            // Assert
            Assert.Equal(SchemaModificationResultEnum.Failure, result.Success);
            Assert.Equal(failureMessage, result.Message);
            mockSchemaRepository.Received(1).TableExists(table.Name);
            mockSchemaRepository.Received(1).CreateTable(table);
        }

        [Fact]
        public void UpsertTargetTable_TableExists_UpdatesTableSuccessfully()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            var table = new Table("account", "Account", false);

            mockSchemaRepository.TableExists(table.Name).Returns(true);
            mockSchemaRepository.UpdateTable(table)
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResultEnum.Success });

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.UpsertTargetTable(table);

            // Assert
            Assert.Equal(SchemaModificationResultEnum.Success, result.Success);
            mockSchemaRepository.Received(1).TableExists(table.Name);
            mockSchemaRepository.Received(1).UpdateTable(table);
            mockSchemaRepository.DidNotReceive().CreateTable(Arg.Any<Table>());
        }

        [Fact]
        public void UpsertTargetTable_TableExists_UpdateFails()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            var table = new Table("account", "Account", false);
            var failureMessage = "Update operation failed";

            mockSchemaRepository.TableExists(table.Name).Returns(true);
            mockSchemaRepository.UpdateTable(table)
                .Returns(new TargetSchemaModificationResult 
                { 
                    Success = SchemaModificationResultEnum.Failure, 
                    Message = failureMessage 
                });

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.UpsertTargetTable(table);

            // Assert
            Assert.Equal(SchemaModificationResultEnum.Failure, result.Success);
            Assert.Equal(failureMessage, result.Message);
            mockSchemaRepository.Received(1).TableExists(table.Name);
            mockSchemaRepository.Received(1).UpdateTable(table);
        }

        [Fact]
        public void UpsertTargetTable_TableExistsCheckThrows_ReturnsFailure()
        {
            // Arrange
            var mockSchemaRepository = Substitute.For<ITargetSchemaRepository>();
            var mockLogger = Substitute.For<ILogger<TargetSchemaService>>();
            var table = new Table("account", "Account", false);
            var exceptionMessage = "Database connection error";

            mockSchemaRepository.When(x => x.TableExists(table.Name))
                .Do(x => throw new InvalidOperationException(exceptionMessage));

            var service = new TargetSchemaService(mockSchemaRepository, mockLogger);

            // Act
            var result = service.UpsertTargetTable(table);

            // Assert
            Assert.Equal(SchemaModificationResultEnum.Failure, result.Success);
            Assert.Equal(exceptionMessage, result.Message);
            mockSchemaRepository.Received(1).TableExists(table.Name);
            mockSchemaRepository.DidNotReceive().CreateTable(Arg.Any<Table>());
            mockSchemaRepository.DidNotReceive().UpdateTable(Arg.Any<Table>());
        }
    }
}
