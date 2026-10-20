using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Source.Tests
{
    public class DataRepositoryTests
    {
        [Fact]
        public void GetRecords_SinglePage_ReturnsAllRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name", "accountid" };

            var entity = new Entity("account");
            entity.Attributes["name"] = "Contoso";
            entity.Attributes["accountid"] = Guid.NewGuid();

            var entityCollection = new EntityCollection(new List<Entity> { entity })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("account", result[0].LogicalName);
            Assert.Equal("Contoso", result[0]["name"]);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetRecords_MultiplePages_ReturnsAllRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name" };

            var entity1 = new Entity("account");
            entity1.Attributes["name"] = "Page1Record";

            var entity2 = new Entity("account");
            entity2.Attributes["name"] = "Page2Record";

            var firstPage = new EntityCollection(new List<Entity> { entity1 })
            {
                MoreRecords = true,
                PagingCookie = "<cookie page=\"1\"/>"
            };

            var secondPage = new EntityCollection(new List<Entity> { entity2 })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>())
                .Returns(firstPage, secondPage); //Sequential return overload

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Page1Record", result[0]["name"]);
            Assert.Equal("Page2Record", result[1]["name"]);
            mockService.Received(2).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetRecords_EmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name" };

            var emptyCollection = new EntityCollection(new List<Entity>())
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(emptyCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetRecords_EntityHasExtraAttributes_OnlyRequestedColumnsReturned()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name" };

            var entity = new Entity("account");
            entity.Attributes["name"] = "Contoso";
            entity.Attributes["revenue"] = 100000m;
            entity.Attributes["createdon"] = DateTime.UtcNow;

            var entityCollection = new EntityCollection(new List<Entity> { entity })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, null);

            // Assert
            Assert.Single(result);
            Assert.True(result[0].Attributes.ContainsKey("name"));
            Assert.False(result[0].Attributes.ContainsKey("revenue"));
            Assert.False(result[0].Attributes.ContainsKey("createdon"));
        }

        [Fact]
        public void GetRecords_WithFilterExpression_ReturnsFilteredRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name", "statecode" };

            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("statecode", ConditionOperator.Equal, 0);

            var activeEntity = new Entity("account");
            activeEntity.Attributes["name"] = "Contoso";
            activeEntity.Attributes["statecode"] = 0;

            var entityCollection = new EntityCollection(new List<Entity> { activeEntity })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Contoso", result[0]["name"]);
            Assert.Equal(0, result[0]["statecode"]);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetRecords_WithFilterExpression_ReturnsMatchingRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();
            var columns = new List<string> { "name", "statecode" };

            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("statecode", ConditionOperator.Equal, 0);

            var activeEntity = new Entity("account");
            activeEntity.Attributes["name"] = "Active Account";
            activeEntity.Attributes["statecode"] = 0;

            var entityCollection = new EntityCollection(new List<Entity> { activeEntity })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns, filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Active Account", result[0]["name"]);
            Assert.Equal(0, result[0]["statecode"]);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetActiveSyncTables_SuccessfulExecute_ReturnsActiveRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var entity1 = new Entity(pg_synctable.EntityLogicalName);
            entity1.Attributes[pg_synctable.Fields.pg_name] = "SyncedTable1";

            var entity2 = new Entity(pg_synctable.EntityLogicalName);
            entity2.Attributes[pg_synctable.Fields.pg_name] = "SyncedTable2";

            var entityCollection = new EntityCollection(new List<Entity> { entity1, entity2 })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetActiveSyncTables();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("SyncedTable1", result[0][pg_synctable.Fields.pg_name]);
            Assert.Equal("SyncedTable2", result[1][pg_synctable.Fields.pg_name]);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetActiveSyncTables_SuccessfulExecute_ReturnsEntitiesWithExpectedAttributes()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var entity = new Entity(pg_synctable.EntityLogicalName);
            entity.Attributes[pg_synctable.Fields.pg_name] = "SyncedTable1";

            var entityCollection = new EntityCollection(new List<Entity> { entity })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetActiveSyncTables();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(pg_synctable.EntityLogicalName, result[0].LogicalName);
            Assert.True(result[0].Attributes.ContainsKey(pg_synctable.Fields.pg_name));
            Assert.Equal("SyncedTable1", result[0][pg_synctable.Fields.pg_name]);
        }

        [Fact]
        public void GetActiveSyncTables_SuccessfulExecute_ReturnsOnlyActiveRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var activeEntity1 = new Entity(pg_synctable.EntityLogicalName);
            activeEntity1.Attributes[pg_synctable.Fields.pg_name] = "ActiveTable1";

            var activeEntity2 = new Entity(pg_synctable.EntityLogicalName);
            activeEntity2.Attributes[pg_synctable.Fields.pg_name] = "ActiveTable2";

            var entityCollection = new EntityCollection(new List<Entity> { activeEntity1, activeEntity2 })
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetActiveSyncTables();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.True(e.Attributes.ContainsKey(pg_synctable.Fields.pg_name)));
        }

        [Fact]
        public void GetActiveSyncTables_NoActiveRecords_ReturnsEmptyList()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var entityCollection = new EntityCollection(new List<Entity>())
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(entityCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetActiveSyncTables();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncDetails_MatchingRecords_ReturnsSyncDetails()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var details1 = new pg_synchronizationdetails(Guid.NewGuid())
            {
                pg_checksum = "checksum1",
                pg_targetrepository = pg_TargetRepositorySet.MicrosoftSqlServer
            };

            var details2 = new pg_synchronizationdetails(Guid.NewGuid())
            {
                pg_checksum = "checksum2",
                pg_targetrepository = pg_TargetRepositorySet.MicrosoftSqlServer
            };

            var entityCollection = new EntityCollection(new List<Entity> { details1, details2 })
            {
                MoreRecords = false
            };

            mockService.Execute(Arg.Any<RetrieveMultipleRequest>()).Returns(new RetrieveMultipleResponse { Results = { ["EntityCollection"] = entityCollection } });

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.pg_checksum == "checksum1");
            Assert.Contains(result, d => d.pg_checksum == "checksum2");
            mockService.Received(1).Execute(Arg.Any<RetrieveMultipleRequest>());
        }

        [Fact]
        public void GetSyncDetails_NoMatchingRecords_ReturnsEmptyList()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var emptyCollection = new EntityCollection(new List<Entity>())
            {
                MoreRecords = false
            };

            mockService.Execute(Arg.Any<RetrieveMultipleRequest>()).Returns(new RetrieveMultipleResponse { Results = { ["EntityCollection"] = emptyCollection } });

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetSyncDetails("nonexistent", pg_TargetRepositorySet.MicrosoftSqlServer);
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockService.Received(1).Execute(Arg.Any<RetrieveMultipleRequest>());
        }

        [Fact]
        public void GetSyncDetails_MultiplePages_ReturnsAllRecords()        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var details1 = new pg_synchronizationdetails(Guid.NewGuid())
            {
                pg_checksum = "page1"
            };

            var details2 = new pg_synchronizationdetails(Guid.NewGuid())
            {
                pg_checksum = "page2"
            };

            var firstPage = new EntityCollection(new List<Entity> { details1 })
            {
                MoreRecords = true,
                PagingCookie = "<cookie page=\"1\"/>"
            };

            var secondPage = new EntityCollection(new List<Entity> { details2 })
            {
                MoreRecords = false
            };

            mockService.Execute(Arg.Any<RetrieveMultipleRequest>()).Returns(
                new RetrieveMultipleResponse { Results = { ["EntityCollection"] = firstPage } },
                new RetrieveMultipleResponse { Results = { ["EntityCollection"] = secondPage } });

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("page1", result[0].pg_checksum);
            Assert.Equal("page2", result[1].pg_checksum);
            mockService.Received(2).Execute(Arg.Any<RetrieveMultipleRequest>());
        }

        [Fact]
        public void GetSyncTable_ExistingName_ReturnsSyncTable()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var syncTable = new pg_synctable(Guid.NewGuid())
            {
                pg_name = "account"
            };

            var entityCollection = new EntityCollection(new List<Entity> { syncTable })
            {
                MoreRecords = false
            };

            mockService.Execute(Arg.Any<RetrieveMultipleRequest>()).Returns(
                new RetrieveMultipleResponse { Results = { ["EntityCollection"] = entityCollection } });

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetSyncTable("account");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(syncTable.Id, result.Id);
            Assert.Equal("account", result.pg_name);
            mockService.Received(1).Execute(Arg.Any<RetrieveMultipleRequest>());
        }

        [Fact]
        public void GetSyncTable_UnknownName_ReturnsNull()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataRepository>>();

            var emptyCollection = new EntityCollection(new List<Entity>())
            {
                MoreRecords = false
            };

            mockService.Execute(Arg.Any<RetrieveMultipleRequest>()).Returns(
                new RetrieveMultipleResponse { Results = { ["EntityCollection"] = emptyCollection } });

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetSyncTable("unknown");

            // Assert
            Assert.Null(result);
            mockService.Received(1).Execute(Arg.Any<RetrieveMultipleRequest>());
        }
    }
}
