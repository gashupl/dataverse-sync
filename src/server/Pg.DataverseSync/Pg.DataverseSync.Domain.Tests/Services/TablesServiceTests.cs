using Moq;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Domain.Tests.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pg.DataverseSync.Domain.Tests.Services
{
    public class TablesServiceTests : ServiceTestBase
    {
        private List<Table> _allTables = new List<Table>
        {
            new Table { Name = "Test 1", SchemaName = "pg_test1" },
            new Table { Name = "Test 2", SchemaName = "pg_test2" },
            new Table { Name = "Test 3", SchemaName = "pg_test3" }
        };
        [Fact]
        public void GetUnsynchronizedTables_NoSyncTable_ReturnsAllStandardTables()
        {
            // Arrange
            var repo = new Mock<IRepository>(); 
            repo.Setup(r => r.GetActiveSynchronizedTables()).Returns(
                new List<pg_synctable>());
            repo.Setup(r => r.GetStandardTablesFromMetadata()).Returns(_allTables);

            var tablesService = new TablesService(repo.Object, this.tracingService);
            
            // Act
            var unsynchronizedTables = tablesService.GetUnsynchronizedTables();

            // Assert
            Assert.NotNull(unsynchronizedTables); 
            Assert.Equal(_allTables.Count, unsynchronizedTables.Count);
        }

        [Fact]
        public void GetUnsynchronizedTables_SyncTableAvailable_ReturnsFilteredStandardTables()
        {
            // Arrange
            var expectedTablesCount = 2; 
            var repo = new Mock<IRepository>();
            repo.Setup(r => r.GetActiveSynchronizedTables()).Returns(
                new List<pg_synctable>() { new pg_synctable { Id = Guid.NewGuid(), pg_name = "pg_test1" } });
            repo.Setup(r => r.GetStandardTablesFromMetadata()).Returns(_allTables);

            var tablesService = new TablesService(repo.Object, this.tracingService);

            // Act
            var unsynchronizedTables = tablesService.GetUnsynchronizedTables();

            // Assert
            Assert.NotNull(unsynchronizedTables);
            Assert.Equal(expectedTablesCount, unsynchronizedTables.Count);
        }


    }
}

