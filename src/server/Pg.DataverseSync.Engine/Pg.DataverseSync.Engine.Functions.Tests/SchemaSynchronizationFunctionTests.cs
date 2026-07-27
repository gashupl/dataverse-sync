using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Model;
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
            var syncMetadataService = Substitute.For<ISourceMetadataService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SchemaSynchronizationFunction(syncMetadataService, null!));
        }

        [Fact]
        public void Run_WithTables_CallsGetTablesOnce()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISourceMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            var tables = new List<Table>
            {
                new("account", "Account", false),
                new("contact", "Contact", false)
            };

            syncMetadataService.GetTables().Returns(tables);

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert (check if method has been executed once)
            syncMetadataService.Received(1).GetTables();
        }

        [Fact]
        public void Run_NullTablesResult_SkipsSynchronization()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISourceMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.GetTables().Returns((List<Table>?)null);

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert (check if method has been executed once)
            syncMetadataService.Received(1).GetTables();
        }

        [Fact]
        public void Run_EmptyTablesList_SkipsSynchronization()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SchemaSynchronizationFunction>>();
            var syncMetadataService = Substitute.For<ISourceMetadataService>();
            var timer = Substitute.For<TimerInfo>();

            syncMetadataService.GetTables().Returns(new List<Table>());

            var function = new SchemaSynchronizationFunction(syncMetadataService, logger);

            // Act
            function.Run(timer);

            // Assert (check if method has been executed once)
            syncMetadataService.Received(1).GetTables();
        }
    }
}
