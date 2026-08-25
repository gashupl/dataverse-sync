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

            var entity1 = new Entity(SyncTable.EntityName);
            entity1.Attributes[SyncTable.Columns.Name] = "account";

            var entity2 = new Entity(SyncTable.EntityName);
            entity2.Attributes[SyncTable.Columns.Name] = "contact";

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

            var entityWithName = new Entity(SyncTable.EntityName);
            entityWithName.Attributes[SyncTable.Columns.Name] = "account";

            var entityWithNullName = new Entity(SyncTable.EntityName);
            entityWithNullName.Attributes[SyncTable.Columns.Name] = null;

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
    }
}

