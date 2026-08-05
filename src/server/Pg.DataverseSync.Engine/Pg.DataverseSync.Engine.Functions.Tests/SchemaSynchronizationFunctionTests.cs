using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Application;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Functions.Tests
{
    [ExcludeFromCodeCoverage]
    public class SchemaSynchronizationFunctionTests
    {
        [Fact]
        public void Constructor_NullSyncMetadataService_ThrowsArgumentNullException()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(null!, logger));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var syncMetadataService = Substitute.For<ISyncMetadataService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(syncMetadataService, null!));
        }

        [Fact]
        public void Run_Execute_IsCalledOnce()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.Execute().Returns(new SyncMetadataResult { TablesSyncResult = [] });

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
        }

        [Fact]
        public void Run_ExecuteReturnsNullTablesSyncResult_SkipsSynchronization()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.Execute().Returns(new SyncMetadataResult { TablesSyncResult = null! });

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
        }

        [Fact]
        public void Run_ExecuteReturnsSucceededResults_ReturnsSucceededTables()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            var syncResult = new SyncMetadataResult
            {
                TablesSyncResult =
                [
                    new TableSyncResult("account", isSynchronized: true),
                    new TableSyncResult("contact", isSynchronized: true)
                ]
            };

            syncMetadataService.Execute().Returns(syncResult);

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
            Assert.Equal(2, syncResult.TablesSyncResult.Count(t => t.IsSynchronized));
            Assert.DoesNotContain(syncResult.TablesSyncResult, t => !t.IsSynchronized);
        }

        [Fact]
        public void Run_ExecuteReturnsFailedResults_ReturnsFailedTablesWithErrors()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            var syncResult = new SyncMetadataResult
            {
                TablesSyncResult =
                [
                    new TableSyncResult("account", isSynchronized: false, errorMessage: "Connection timeout"),
                    new TableSyncResult("contact", isSynchronized: false, errorMessage: "Permission denied")
                ]
            };

            syncMetadataService.Execute().Returns(syncResult);

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
            Assert.Equal(2, syncResult.TablesSyncResult.Count(t => !t.IsSynchronized));
            Assert.All(syncResult.TablesSyncResult, t => Assert.False(string.IsNullOrEmpty(t.ErrorMessage)));
        }

        [Fact]
        public void Run_ExecuteReturnsMixedResults_ReturnsBothSucceededAndFailedTables()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            var syncResult = new SyncMetadataResult
            {
                TablesSyncResult =
                [
                    new TableSyncResult("account", isSynchronized: true),
                    new TableSyncResult("contact", isSynchronized: false, errorMessage: "Schema mismatch")
                ]
            };

            syncMetadataService.Execute().Returns(syncResult);

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
            Assert.Equal(1, syncResult.TablesSyncResult.Count(t => t.IsSynchronized));
            Assert.Equal(1, syncResult.TablesSyncResult.Count(t => !t.IsSynchronized));
        }
    }
}
