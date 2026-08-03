using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Application.Data;

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
    }
}
