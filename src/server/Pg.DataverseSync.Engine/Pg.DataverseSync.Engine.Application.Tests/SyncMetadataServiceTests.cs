using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;

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

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account" });
            sourceMetadataService.GetTables().Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(false);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            targetSchemaService.Received(1).CreateTargetTable(Arg.Is<Table>(t => t.Name == "account"));
            targetSchemaService.DidNotReceive().UpdateTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_TargetTableExistsAndSchemaOutdated_UpdatesTargetTable()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account" });
            sourceMetadataService.GetTables().Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(true);
            targetSchemaService.IsTargetTableSchemaUpToDate(Arg.Is<Table>(t => t.Name == "account")).Returns(false);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            targetSchemaService.Received(1).UpdateTargetTable(Arg.Is<Table>(t => t.Name == "account"));
            targetSchemaService.DidNotReceive().CreateTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_TargetTableExistsAndSchemaUpToDate_DoesNotUpdateTargetTable()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account" });
            sourceMetadataService.GetTables().Returns(new List<Table> { new Table("account", "Account", false) });
            targetSchemaService.TargetTableExists("account").Returns(true);
            targetSchemaService.IsTargetTableSchemaUpToDate(Arg.Is<Table>(t => t.Name == "account")).Returns(true);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            targetSchemaService.DidNotReceive().UpdateTargetTable(Arg.Any<Table>());
            targetSchemaService.DidNotReceive().CreateTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_SourceTableMissing_AddsFailedResult()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account" });
            sourceMetadataService.GetTables().Returns(new List<Table> { new Table("contact", "Contact", false) });
            targetSchemaService.TargetTableExists("account").Returns(true);

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Single(result.TablesSyncResult);
            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("Table is missing in source metadata.", result.TablesSyncResult[0].ErrorMessage);
            targetSchemaService.DidNotReceive().CreateTargetTable(Arg.Any<Table>());
            targetSchemaService.DidNotReceive().UpdateTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void Execute_TableSynchronizationFails_ContinuesWithNextTable()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var logger = Substitute.For<ILogger<SyncMetadataService>>();

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account", "contact" });
            sourceMetadataService.GetTables().Returns(new List<Table>
            {
                new Table("account", "Account", false),
                new Table("contact", "Contact", false)
            });
            targetSchemaService.TargetTableExists(Arg.Any<string>()).Returns(false);
            targetSchemaService
                .When(x => x.CreateTargetTable(Arg.Is<Table>(t => t.Name == "account")))
                .Do(_ => throw new InvalidOperationException("creation failed"));

            var service = new SyncMetadataService(sourceMetadataService, targetSchemaService, logger);

            // Act
            var result = service.Execute();

            // Assert
            Assert.Equal(2, result.TablesSyncResult.Count);

            var accountResult = result.TablesSyncResult.Single(x => x.TableName == "account");
            Assert.False(accountResult.IsSynchronized);
            Assert.Equal("creation failed", accountResult.ErrorMessage);

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

            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string> { "account" });
            sourceMetadataService.GetTables().Returns(new List<Table>
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
    }
}
