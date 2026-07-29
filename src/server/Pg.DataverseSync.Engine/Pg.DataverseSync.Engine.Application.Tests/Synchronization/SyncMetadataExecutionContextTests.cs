using Pg.DataverseSync.Engine.Application.Synchronization;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    public class SyncMetadataExecutionContextTests
    {
        [Fact]
        public void Constructor_InitializesDefaultState()
        {
            // Act
            var context = new SyncMetadataExecutionContext();

            // Assert
            Assert.NotNull(context.SynchronizedTableNames);
            Assert.Empty(context.SynchronizedTableNames);
            Assert.Null(context.SourceTables);
            Assert.NotNull(context.TargetTableExists);
            Assert.Empty(context.TargetTableExists);
        }

        [Fact]
        public void TargetTableExists_UsesCaseInsensitiveComparer()
        {
            // Arrange
            var context = new SyncMetadataExecutionContext();

            // Act
            context.TargetTableExists["account"] = true;

            // Assert
            Assert.True(context.TargetTableExists.ContainsKey("ACCOUNT"));
        }
    }
}
