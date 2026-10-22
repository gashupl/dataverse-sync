using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class SourceMetadataServiceTests
    {
        [Fact]
        public void GetTables_SuccessfullRequest_ReturnsTablesWithColumns()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var tables = new List<Table>
            {
                new Table("account", "Account", false)
                {
                    Columns = new List<Column>
                    {
                        new Column("accountid", "Guid", true),
                        new Column("name", "String")
                    }
                },
                new Table("contact", "Contact", false)
                {
                    Columns = new List<Column>
                    {
                        new Column("contactid", "Guid", true),
                        new Column("firstname", "String"),
                        new Column("lastname", "String")
                    }
                },
                new Table("opportunity", "Opportunity", false)
                {
                    Columns = new List<Column>
                    {
                        new Column("opportunityid", "Guid", true),
                        new Column("name", "String"),
                        new Column("estimatedvalue", "Double")
                    }
                }
            };

            mockMetadataRepo.GetTables().Returns(tables);
            mockMetadataRepo.GetColumns("account").Returns(tables[0].Columns);
            mockMetadataRepo.GetColumns("contact").Returns(tables[1].Columns);
            mockMetadataRepo.GetColumns("opportunity").Returns(tables[2].Columns);  

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            var tableNames = new List<string> { "account", "contact", "opportunity" };

            // Act
            var result = service.GetTables(tableNames);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("account", result[0].Name);
            Assert.Equal("contact", result[1].Name);
            Assert.Equal("opportunity", result[2].Name);

            Assert.All(result, table => Assert.NotEmpty(table.Columns));
            Assert.Equal(2, result[0].Columns.Count);
            Assert.Equal(3, result[1].Columns.Count);
            Assert.Equal(3, result[2].Columns.Count);

            mockMetadataRepo.Received(1).GetTables();
        }

        [Fact]
        public void GetTables_SuccessfullRequestReturnsNull_ReturnsNull()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            mockMetadataRepo.GetTables().Returns((List<Table>?)null);

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            var tableNames = new List<string> { "account" };

            // Act
            var result = service.GetTables(tableNames);

            // Assert
            Assert.Null(result);

            mockMetadataRepo.Received(1).GetTables();
        }

        [Fact]
        public void GetTablesNames_ReadMetadataException_ThrowsDomainServiceException()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var readException = new ReadMetadataException("Failed to read metadata from source.");
            mockMetadataRepo.GetTables().Throws(readException);

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            var tableNames = new List<string> { "account" };

            // Act & Assert
            var exception = Assert.Throws<ApplicationServiceException>(() => service.GetTables(tableNames));
            
            Assert.Equal("An error occurred while reading metadata for tables.", exception.Message);
            Assert.Equal(readException, exception.InnerException);

            mockMetadataRepo.Received(1).GetTables();
        }

        [Fact]
        public void GetSynchronizedTableNames_SuccessfulRequest_ReturnsTableNames()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var entity1 = new Entity(pg_synctable.EntityLogicalName);
            entity1.Attributes[pg_synctable.Fields.pg_name] = "account";

            var entity2 = new Entity(pg_synctable.EntityLogicalName);
            entity2.Attributes[pg_synctable.Fields.pg_name] = "contact";

            mockDataRepository.GetActiveSyncTables().Returns(new List<Entity> { entity1, entity2 });

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.GetSynchronizedTableNames();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("account", result[0]);
            Assert.Equal("contact", result[1]);

            mockDataRepository.Received(1).GetActiveSyncTables();
        }

        [Fact]
        public void GetSynchronizedTableNames_EntityNameIsNull_ReturnsOnlyNotNullNames()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var entityWithName = new Entity(pg_synctable.EntityLogicalName);
            entityWithName.Attributes[pg_synctable.Fields.pg_name] = "account";

            var entityWithNullName = new Entity(pg_synctable.EntityLogicalName);
            entityWithNullName.Attributes[pg_synctable.Fields.pg_name] = null;

            mockDataRepository.GetActiveSyncTables().Returns(new List<Entity> { entityWithName, entityWithNullName });

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.GetSynchronizedTableNames();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("account", result[0]);

            mockDataRepository.Received(1).GetActiveSyncTables();
        }

        [Fact]
        public void UpsertSyncDetails_ExistingDetails_UpdatesRecordAndReturnsSuccess()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var detailsId = Guid.NewGuid();
            var existingDetails = new pg_synchronizationdetails(detailsId)
            {
                pg_checksum = "old-checksum"
            };

            mockDataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails> { existingDetails });

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.UpsertSyncDetails("account", "new-checksum", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.Equal(ModificationResult.Success, result.Success);
            mockDataRepository.Received(1).UpdateRecord(Arg.Is<Entity>(e =>
                e.Id == detailsId && ((pg_synchronizationdetails)e).pg_checksum == "new-checksum"));
            mockDataRepository.DidNotReceive().CreateRecord(Arg.Any<Entity>());
            mockDataRepository.DidNotReceive().GetSyncTable(Arg.Any<string>());
        }

        [Fact]
        public void UpsertSyncDetails_NoExistingDetails_CreatesRecordLinkedToSyncTableAndReturnsSuccess()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var syncTableId = Guid.NewGuid();
            var syncTable = new pg_synctable(syncTableId)
            {
                pg_name = "account"
            };

            mockDataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails>());
            mockDataRepository.GetSyncTable("account").Returns(syncTable);

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.UpsertSyncDetails("account", "new-checksum", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.Equal(ModificationResult.Success, result.Success);
            mockDataRepository.Received(1).CreateRecord(Arg.Is<Entity>(e =>
                ((pg_synchronizationdetails)e).pg_checksum == "new-checksum"
                && ((pg_synchronizationdetails)e).pg_targetrepository == pg_TargetRepositorySet.MicrosoftSqlServer
                && ((pg_synchronizationdetails)e).pg_synctableId.Id == syncTableId));
            mockDataRepository.DidNotReceive().UpdateRecord(Arg.Any<Entity>());
        }

        [Fact]
        public void UpsertSyncDetails_NoExistingDetailsAndSyncTableNotFound_ReturnsFailureWithoutCreating()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            mockDataRepository.GetSyncDetails("unknown", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails>());
            mockDataRepository.GetSyncTable("unknown").Returns((pg_synctable?)null);

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.UpsertSyncDetails("unknown", "new-checksum", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.Equal(ModificationResult.Failure, result.Success);
            Assert.Equal("Sync table 'unknown' was not found.", result.Message);
            mockDataRepository.DidNotReceive().CreateRecord(Arg.Any<Entity>());
            mockDataRepository.DidNotReceive().UpdateRecord(Arg.Any<Entity>());
        }

        [Fact]
        public void UpsertSyncDetails_RepositoryThrows_ReturnsFailureWithExceptionMessage()
        {
            // Arrange
            var mockMetadataRepo = Substitute.For<IMetadataRepository>();
            var mockDataRepository = Substitute.For<IDataRepository>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            mockDataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Throws(new Exception("Dataverse unavailable"));

            var service = new SourceMetadataService(mockMetadataRepo, mockDataRepository, mockLogger);

            // Act
            var result = service.UpsertSyncDetails("account", "new-checksum", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.Equal(ModificationResult.Failure, result.Success);
            Assert.Equal("Dataverse unavailable", result.Message);
            mockDataRepository.DidNotReceive().CreateRecord(Arg.Any<Entity>());
            mockDataRepository.DidNotReceive().UpdateRecord(Arg.Any<Entity>());
        }
    }
}

