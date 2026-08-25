using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class SyncMetadataServiceTests
    {
        [Fact]
        public void Execute_TargetTableDoesNotExist_CreatesTargetTableAndReturnsSuccess()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(false);
            targetSchemaService.UpsertTargetTable(Arg.Any<Table>())
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResult.Success });

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            targetSchemaService.Received(1).UpsertTargetTable(Arg.Is<Table>(t => t.Name == "account"));
        }

        [Fact]
        public void Execute_TargetTableExistsAndSchemaUpToDate_DoesNotUpdateTargetTable()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(true);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            targetSchemaService.DidNotReceive().UpsertTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_SourceTableMissing_AddsFailedResult()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table> { new Table("contact", "Contact", false) });
            targetSchemaService.TargetTableExists("account").Returns(true);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("Table is missing in source metadata.", result.TablesSyncResult[0].ErrorMessage);
            targetSchemaService.DidNotReceive().UpsertTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_TableSynchronizationFails_ContinuesWithNextTable()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var tableNames = new List<string> { "account", "contact" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table>
            {
                new Table("account", "Account", false),
                new Table("contact", "Contact", false)
            });
            targetSchemaService.TargetTableExists(Arg.Any<string>()).Returns(false);

            // Setup first table to fail, second to succeed
            var failureMessage = "creation failed";
            targetSchemaService.UpsertTargetTable(Arg.Is<Table>(t => t.Name == "account"))
                .Returns(new TargetSchemaModificationResult 
                { 
                    Success = SchemaModificationResult.Failure, 
                    Message = failureMessage 
                });
            targetSchemaService.UpsertTargetTable(Arg.Is<Table>(t => t.Name == "contact"))
                .Returns(new TargetSchemaModificationResult 
                { 
                    Success = SchemaModificationResult.Success 
                });

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Equal(2, result.TablesSyncResult.Count);

            var accountResult = result.TablesSyncResult.Single(x => x.TableName == "account");
            Assert.False(accountResult.IsSynchronized);
            Assert.Equal(failureMessage, accountResult.ErrorMessage);

            var contactResult = result.TablesSyncResult.Single(x => x.TableName == "contact");
            Assert.True(contactResult.IsSynchronized);
        }

        [Fact]
        public void Execute_HandlerThrows_RethrowsApplicationServiceException()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var innerException = new InvalidOperationException("source failure");
            sourceMetadataService.GetSynchronizedTableNames().Throws(innerException);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var exception = Assert.Throws<ApplicationServiceException>(() => service.Execute());

            // Assert
            Assert.Equal("An error occurred while retrieving synchronized table names.", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void Execute_UnexpectedException_WrapsInApplicationServiceException()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table>
            {
                new Table(null!, "Broken", false)
            });
            targetSchemaService.TargetTableExists("account").Returns(true);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var exception = Assert.Throws<ApplicationServiceException>(() => service.Execute());

            // Assert
            Assert.Equal("An error occurred while executing metadata synchronization.", exception.Message);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<ArgumentNullException>(exception.InnerException);
        }

        [Fact]
        public void Execute_UpsertTargetTableReturnsFailure_AddsFailedResultWithErrorMessage()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();
            var errorMessage = "Table creation failed: Column mismatch detected";

            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetSynchronizedTableNames().Returns(tableNames);
            sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(false);
            targetSchemaService.UpsertTargetTable(Arg.Any<Table>())
                .Returns(new TargetSchemaModificationResult 
                { 
                    Success = SchemaModificationResult.Failure, 
                    Message = errorMessage 
                });

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal(errorMessage, result.TablesSyncResult[0].ErrorMessage);
            targetSchemaService.Received(1).UpsertTargetTable(Arg.Is<Table>(t => t.Name == "account"));
        }
    }
}
