using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
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
            var result = repository.GetRecords("account", columns);

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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
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
            var result = repository.GetRecords("account", columns);

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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
            var columns = new List<string> { "name" };

            var emptyCollection = new EntityCollection(new List<Entity>())
            {
                MoreRecords = false
            };

            mockService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(emptyCollection);

            var repository = new DataRepository(mockService, mockLogger);

            // Act
            var result = repository.GetRecords("account", columns);

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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
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
            var result = repository.GetRecords("account", columns);

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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();
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
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();

            var entity1 = new Entity(SyncTable.EntityName);
            entity1.Attributes[SyncTable.Columns.Name] = "SyncedTable1";

            var entity2 = new Entity(SyncTable.EntityName);
            entity2.Attributes[SyncTable.Columns.Name] = "SyncedTable2";

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
            Assert.Equal("SyncedTable1", result[0][SyncTable.Columns.Name]);
            Assert.Equal("SyncedTable2", result[1][SyncTable.Columns.Name]);
            mockService.Received(1).RetrieveMultiple(Arg.Any<QueryExpression>());
        }

        [Fact]
        public void GetActiveSyncTables_SuccessfulExecute_ReturnsEntitiesWithExpectedAttributes()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();

            var entity = new Entity(SyncTable.EntityName);
            entity.Attributes[SyncTable.Columns.Name] = "SyncedTable1";

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
            Assert.Equal(SyncTable.EntityName, result[0].LogicalName);
            Assert.True(result[0].Attributes.ContainsKey(SyncTable.Columns.Name));
            Assert.Equal("SyncedTable1", result[0][SyncTable.Columns.Name]);
        }

        [Fact]
        public void GetActiveSyncTables_SuccessfulExecute_ReturnsOnlyActiveRecords()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();

            var activeEntity1 = new Entity(SyncTable.EntityName);
            activeEntity1.Attributes[SyncTable.Columns.Name] = "ActiveTable1";

            var activeEntity2 = new Entity(SyncTable.EntityName);
            activeEntity2.Attributes[SyncTable.Columns.Name] = "ActiveTable2";

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
            Assert.All(result, e => Assert.True(e.Attributes.ContainsKey(SyncTable.Columns.Name)));
        }

        [Fact]
        public void GetActiveSyncTables_NoActiveRecords_ReturnsEmptyList()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<DataverseRepositoryBase>>();

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
    }
}
