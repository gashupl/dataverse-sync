using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Synchronization;
using Pg.DataverseSync.Engine.Core.Exceptions;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    public class CheckTargetSchemaHandlerTests
    {
        [Fact]
        public void Handle_ExistingAndMissingTables_PopulatesTargetExistenceMap()
        {
            // Arrange
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            targetSchemaService.TargetTableExists("account").Returns(true);
            targetSchemaService.TargetTableExists("contact").Returns(false);

            var handler = new CheckTargetSchemaHandler(targetSchemaService);
            var context = new SyncMetadataExecutionContext
            {
                SynchronizedTableNames = new List<string> { "account", "contact" }
            };

            // Act
            handler.Handle(context);

            // Assert
            Assert.True(context.TargetTableExists["account"]);
            Assert.False(context.TargetTableExists["contact"]);
            targetSchemaService.Received(1).TargetTableExists("account");
            targetSchemaService.Received(1).TargetTableExists("contact");
        }

        [Fact]
        public void Handle_TargetServiceThrows_ThrowsApplicationServiceException()
        {
            // Arrange
            var targetSchemaService = Substitute.For<ITargetSchemaService>();
            var innerException = new InvalidOperationException("target error");
            targetSchemaService.TargetTableExists(Arg.Any<string>()).Throws(innerException);

            var handler = new CheckTargetSchemaHandler(targetSchemaService);
            var context = new SyncMetadataExecutionContext
            {
                SynchronizedTableNames = new List<string> { "account" }
            };

            // Act
            var exception = Assert.Throws<ApplicationServiceException>(() => handler.Handle(context));

            // Assert
            Assert.Equal("An error occurred while checking target schema for synchronized tables.", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
    }
}
