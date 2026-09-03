using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Synchronization;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    public class RetrieveSourceTablesHandlerTests
    {
        [Fact]
        public void Handle_ServiceReturnsTables_PopulatesContext()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var tables = new List<Table>
            {
                new Table("account", "Account", false),
                new Table("contact", "Contact", false)
            };
            var tableNames = new List<string> { "account", "contact" };
            sourceMetadataService.GetTables(tableNames).Returns(tables);

            var handler = new RetrieveSourceTablesHandler(sourceMetadataService);
            var context = new SyncMetadataExecutionContext()
            {
                SynchronizedTableNames = tableNames
            };

            // Act
            handler.Handle(context);

            // Assert
            Assert.NotNull(context.SourceTables);
            Assert.Equal(2, context.SourceTables.Count);
            Assert.Equal("account", context.SourceTables[0].Name);
        }

        [Fact]
        public void Handle_ServiceReturnsNull_SetsEmptyCollection()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetTables(tableNames).Returns((List<Table>?)null);

            var handler = new RetrieveSourceTablesHandler(sourceMetadataService);
            var context = new SyncMetadataExecutionContext()
            {
                SynchronizedTableNames = tableNames
            };

            // Act
            handler.Handle(context);

            // Assert
            Assert.NotNull(context.SourceTables);
            Assert.Empty(context.SourceTables);
        }

        [Fact]
        public void Handle_ServiceThrows_ThrowsApplicationServiceException()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var innerException = new InvalidOperationException("metadata error");
            var tableNames = new List<string> { "account" };
            sourceMetadataService.GetTables(tableNames).Throws(innerException);

            var handler = new RetrieveSourceTablesHandler(sourceMetadataService);
            var context = new SyncMetadataExecutionContext()
            {
                SynchronizedTableNames = tableNames
            };

            // Act
            var exception = Assert.Throws<ApplicationServiceException>(() => handler.Handle(context));

            // Assert
            Assert.Equal("An error occurred while retrieving source tables metadata.", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
