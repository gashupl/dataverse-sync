using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class SyncMetadataServiceExecuteTableTests
    {
        private readonly ISourceMetadataService _sourceMetadataService = Substitute.For<ISourceMetadataService>();
        private readonly ITargetSchemaService _targetSchemaService = Substitute.For<ITargetSchemaService>();
        private readonly IDataRepository _dataRepository = Substitute.For<IDataRepository>();
        private readonly SyncMetadataService _service;

        public SyncMetadataServiceExecuteTableTests()
        {
            _dataRepository.GetSyncDetails(Arg.Any<string>(), Arg.Any<pg_TargetRepositorySet>())
                .Returns(new List<pg_synchronizationdetails>());

            _service = new SyncMetadataService(_sourceMetadataService, _targetSchemaService, _dataRepository,
                Substitute.For<ILogger<SyncMetadataService>>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ExecuteTable_MissingTableName_ThrowsArgumentException(string? tableName)
        {
            Assert.ThrowsAny<ArgumentException>(() => _service.Execute(tableName!));

            _sourceMetadataService.DidNotReceiveWithAnyArgs().GetTables(default!);
        }

        [Fact]
        public void ExecuteTable_ValidTable_SynchronizesOnlyRequestedTable()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>())
                .Returns(new List<Table> { new Table("account", "Account", false) });
            _targetSchemaService.UpsertTargetTable(Arg.Any<Table>())
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResult.Success });
            _sourceMetadataService.UpsertSyncDetails(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<pg_TargetRepositorySet>())
                .Returns(new SyncDetailsModificationResult { Success = ModificationResult.Success });

            var result = _service.Execute("account");

            Assert.Single(result.TablesSyncResult);
            Assert.Equal("account", result.TablesSyncResult[0].TableName);
            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            _sourceMetadataService.Received(1).GetTables(Arg.Is<List<string>>(names => names.Count == 1 && names[0] == "account"));
            _sourceMetadataService.DidNotReceive().GetSynchronizedTableNames();
            _targetSchemaService.Received(1).UpsertTargetTable(Arg.Is<Table>(t => t.Name == "account"));
        }

        [Fact]
        public void ExecuteTable_SourceTableMissing_ReturnsFailedResult()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>())
                .Returns(new List<Table> { new Table("contact", "Contact", false) });

            var result = _service.Execute("account");

            Assert.Single(result.TablesSyncResult);
            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("Table is missing in source metadata.", result.TablesSyncResult[0].ErrorMessage);
            _targetSchemaService.DidNotReceive().UpsertTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void ExecuteTable_SourceReturnsNull_ReturnsFailedResult()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns((List<Table>?)null);

            var result = _service.Execute("account");

            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("Table is missing in source metadata.", result.TablesSyncResult[0].ErrorMessage);
        }

        [Fact]
        public void ExecuteTable_ChecksumsMatch_DoesNotUpsertTargetTable()
        {
            var table = new Table("account", "Account", false)
            {
                Columns = new List<Column> { new Column("accountid", "Guid", true) }
            };
            _sourceMetadataService.GetTables(Arg.Any<List<string>>()).Returns(new List<Table> { table });
            _dataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails>
                {
                    new pg_synchronizationdetails(Guid.NewGuid()) { pg_checksum = table.GetChecksum() }
                });

            var result = _service.Execute("account");

            Assert.True(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal(TableSyncOperationCode.None, result.TablesSyncResult[0].OperationCode);
            _targetSchemaService.DidNotReceive().UpsertTargetTable(Arg.Any<Table>());
        }

        [Fact]
        public void ExecuteTable_UpsertTargetTableFails_ReturnsFailedResultAndSkipsSyncDetails()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>())
                .Returns(new List<Table> { new Table("account", "Account", false) });
            _targetSchemaService.UpsertTargetTable(Arg.Any<Table>())
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResult.Failure, Message = "failed" });

            var result = _service.Execute("account");

            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("failed", result.TablesSyncResult[0].ErrorMessage);
            _sourceMetadataService.DidNotReceive().UpsertSyncDetails(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<pg_TargetRepositorySet>());
        }

        [Fact]
        public void ExecuteTable_UpsertSyncDetailsFails_ReturnsFailedResult()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>())
                .Returns(new List<Table> { new Table("account", "Account", false) });
            _targetSchemaService.UpsertTargetTable(Arg.Any<Table>())
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResult.Success });
            _sourceMetadataService.UpsertSyncDetails(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<pg_TargetRepositorySet>())
                .Returns(new SyncDetailsModificationResult { Success = ModificationResult.Failure, Message = "details failed" });

            var result = _service.Execute("account");

            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("details failed", result.TablesSyncResult[0].ErrorMessage);
        }

        [Fact]
        public void ExecuteTable_UpsertTargetTableThrows_ReturnsFailedResultWithExceptionMessage()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>())
                .Returns(new List<Table> { new Table("account", "Account", false) });
            _targetSchemaService.UpsertTargetTable(Arg.Any<Table>()).Throws(new InvalidOperationException("boom"));

            var result = _service.Execute("account");

            Assert.False(result.TablesSyncResult[0].IsSynchronized);
            Assert.Equal("boom", result.TablesSyncResult[0].ErrorMessage);
        }

        [Fact]
        public void ExecuteTable_SourceServiceThrows_ThrowsApplicationServiceException()
        {
            _sourceMetadataService.GetTables(Arg.Any<List<string>>()).Throws(new InvalidOperationException("source error"));

            var exception = Assert.Throws<ApplicationServiceException>(() => _service.Execute("account"));

            Assert.Equal("An error occurred while retrieving source tables metadata.", exception.Message);
        }
    }
}
