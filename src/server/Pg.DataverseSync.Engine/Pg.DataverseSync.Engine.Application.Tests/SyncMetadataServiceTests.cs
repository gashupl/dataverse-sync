using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Application.Source;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class SyncMetadataServiceTests
    {
        [Fact]
        public void GetTables_SuccessfullRequest_ReturnsTables()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SyncMetadataService>>();

            var tables = new List<Table>
            {
                new Table("account", "Account", false),
                new Table("contact", "Contact", false),
                new Table("opportunity", "Opportunity", false)
            };

            mockMetadataReader.GetTables().Returns(tables);

            var service = new SyncMetadataService(mockMetadataReader, mockLogger);

            // Act
            var result = service.GetTables();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("account", result[0].Name);
            Assert.Equal("contact", result[1].Name);
            Assert.Equal("opportunity", result[2].Name);

            mockMetadataReader.Received(1).GetTables();
        }

        [Fact]
        public void GetTables_SuccessfullRequestReturnsNull_ReturnsNull()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SyncMetadataService>>();

            mockMetadataReader.GetTables().Returns((List<Table>?)null);

            var service = new SyncMetadataService(mockMetadataReader, mockLogger);

            // Act
            var result = service.GetTables();

            // Assert
            Assert.Null(result);

            mockMetadataReader.Received(1).GetTables();
        }

        [Fact]
        public void GetTablesNames_ReadMetadataException_ThrowsDomainServiceException()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SyncMetadataService>>();

            var readException = new ReadMetadataException("Failed to read metadata from source.");
            mockMetadataReader.GetTables().Throws(readException);

            var service = new SyncMetadataService(mockMetadataReader, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ApplicationServiceException>(() => service.GetTables());
            
            Assert.Equal("An error occurred while reading metadata for tables.", exception.Message);
            Assert.Equal(readException, exception.InnerException);

            mockMetadataReader.Received(1).GetTables();
        }
    }
}

