using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Domain.Source;

namespace Pg.DataverseSync.Engine.Domain.Tests
{
    public class SourceMetadataServiceTests
    {
        [Fact]
        public void GetTablesNames_SuccessfullRequest_ReturnsTableNames()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var tables = new List<Table>
            {
                new Table("account", "Account", false),
                new Table("contact", "Contact", false),
                new Table("opportunity", "Opportunity", false)
            };

            mockMetadataReader.GetTables().Returns(tables);

            var service = new SourceMetadataService(mockMetadataReader, mockLogger);

            // Act
            var result = service.GetTablesNames();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("account", result[0]);
            Assert.Equal("contact", result[1]);
            Assert.Equal("opportunity", result[2]);

            mockMetadataReader.Received(1).GetTables();
        }

        [Fact]
        public void GetTablesNames_SuccessfullRequestReturnsNull_ReturnsEmptyList()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            mockMetadataReader.GetTables().Returns((List<Table>?)null);

            var service = new SourceMetadataService(mockMetadataReader, mockLogger);

            // Act
            var result = service.GetTablesNames();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            mockMetadataReader.Received(1).GetTables();
        }

        [Fact]
        public void GetTablesNames_ReadMetadataException_ThrowsDomainServiceException()
        {
            // Arrange
            var mockMetadataReader = Substitute.For<IMetadataReader>();
            var mockLogger = Substitute.For<ILogger<SourceMetadataService>>();

            var readException = new ReadMetadataException("Failed to read metadata from source.");
            mockMetadataReader.GetTables().Throws(readException);

            var service = new SourceMetadataService(mockMetadataReader, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<DomainServiceException>(() => service.GetTablesNames());
            
            Assert.Equal("An error occurred while reading metadata for table names.", exception.Message);
            Assert.Equal(readException, exception.InnerException);

            mockMetadataReader.Received(1).GetTables();
        }
    }
}

