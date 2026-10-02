using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Schema;
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
            var dataLoadService = Substitute.For<IDataLoadService>();
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(null!, dataLoadService, logger));
        }

        [Fact]
        public void Constructor_NullDataLoadService_ThrowsArgumentNullException()
        {
            // Arrange
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(syncMetadataService, null!, logger));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var dataLoadService = Substitute.For<IDataLoadService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(syncMetadataService, dataLoadService, null!));
        }

        [Fact]
        public void Run_Execute_IsCalledOnce()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var dataLoadService = Substitute.For<IDataLoadService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.Execute().Returns(new SyncMetadataResult { TablesSyncResult = [] });

            var function = new SchemaSynchronizationFunction(syncMetadataService, dataLoadService, logger);

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
            var dataLoadService = Substitute.For<IDataLoadService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.Execute().Returns(new SyncMetadataResult { TablesSyncResult = null! });

            var function = new SchemaSynchronizationFunction(syncMetadataService, dataLoadService, logger);

            // Act
            function.Run(timer);

            // Assert
            syncMetadataService.Received(1).Execute();
            dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
        }

        [Fact]
        public void Run_CreateOperation_LoadsInitialDataOnce()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var dataLoadService = Substitute.For<IDataLoadService>();
            var timer = Substitute.For<TimerInfo>();

            var syncResult = new SyncMetadataResult
            {
                TablesSyncResult =
                [
                    new TableSyncResult("account", true, TableSyncOperationCode.Create)
                ]
            };

            syncMetadataService.Execute().Returns(syncResult);
            dataLoadService.LoadInitialData("account").Returns(
            [
                new Pg.DataverseSync.Engine.Target.TargetRecordModificationResult { Success = true },
                new Pg.DataverseSync.Engine.Target.TargetRecordModificationResult { Success = false, Message = "Insert failed" }
            ]);

            var function = new SchemaSynchronizationFunction(syncMetadataService, dataLoadService, logger);

            // Act
            function.Run(timer);

            // Assert
            dataLoadService.Received(1).LoadInitialData("account");
        }

        [Fact]
        public void Run_NonCreateOperation_DoesNotLoadInitialData()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISyncMetadataService>();
            var dataLoadService = Substitute.For<IDataLoadService>();
            var timer = Substitute.For<TimerInfo>();

            var syncResult = new SyncMetadataResult
            {
                TablesSyncResult =
                [
                    new TableSyncResult("account", true, TableSyncOperationCode.Update),
                    new TableSyncResult("contact", false, errorMessage: "Schema mismatch")
                ]
            };

            syncMetadataService.Execute().Returns(syncResult);

            var function = new SchemaSynchronizationFunction(syncMetadataService, dataLoadService, logger);

            // Act
            function.Run(timer);

            // Assert
            dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
            Assert.Equal(1, syncResult.TablesSyncResult.Count(t => t.IsSynchronized));
            Assert.Equal(1, syncResult.TablesSyncResult.Count(t => !t.IsSynchronized));
        }
    }
}
