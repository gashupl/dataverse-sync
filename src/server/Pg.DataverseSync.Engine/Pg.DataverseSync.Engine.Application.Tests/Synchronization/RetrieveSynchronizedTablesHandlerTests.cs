using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Synchronization;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    public class RetrieveSynchronizedTablesHandlerTests
    {
        [Fact]
        public void Handle_ValidNames_FiltersEmptyAndDistinctCaseInsensitive()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            sourceMetadataService.GetSynchronizedTableNames().Returns(new List<string>
            {
                "account",
                "",
                " ",
                "ACCOUNT",
                "contact"
            });

            var handler = new RetrieveSynchronizedTablesHandler(sourceMetadataService);
            var context = new SyncMetadataExecutionContext();

            // Act
            handler.Handle(context);

            // Assert
            Assert.Equal(2, context.SynchronizedTableNames.Count);
            Assert.Contains("account", context.SynchronizedTableNames, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("contact", context.SynchronizedTableNames, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Handle_SourceServiceThrows_ThrowsApplicationServiceException()
        {
            // Arrange
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var innerException = new InvalidOperationException("source error");
            sourceMetadataService.GetSynchronizedTableNames().Throws(innerException);

            var handler = new RetrieveSynchronizedTablesHandler(sourceMetadataService);
            var context = new SyncMetadataExecutionContext();

            // Act
            var exception = Assert.Throws<ApplicationServiceException>(() => handler.Handle(context));

            // Assert
            Assert.Equal("An error occurred while retrieving synchronized table names.", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
