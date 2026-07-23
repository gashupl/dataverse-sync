using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

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
    }
}
