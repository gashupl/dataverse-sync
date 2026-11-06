using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class SyncMetadataServicePurgeTableTests
    {
        private readonly ISourceMetadataService _sourceMetadataService = Substitute.For<ISourceMetadataService>();
        private readonly ITargetSchemaService _targetSchemaService = Substitute.For<ITargetSchemaService>();
        private readonly IDataRepository _dataRepository = Substitute.For<IDataRepository>();
        private readonly SyncMetadataService _service;

        public SyncMetadataServicePurgeTableTests()
        {
            _service = new SyncMetadataService(_sourceMetadataService, _targetSchemaService, _dataRepository,
                Substitute.For<ILogger<SyncMetadataService>>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PurgeTable_MissingTableName_ThrowsArgumentException(string? tableName)
        {
            Assert.ThrowsAny<ArgumentException>(() => _service.PurgeTable(tableName!));

            _targetSchemaService.DidNotReceiveWithAnyArgs().DeleteTargetTable(default!);
            _dataRepository.DidNotReceiveWithAnyArgs().DeleteRecord(default, default!);
        }

        [Fact]
        public void PurgeTable_TargetTableExistsAndDeleteSucceeds_DeletesSyncDetailsAndReturnsSuccess()
        {
            var firstId = Guid.NewGuid();
            var secondId = Guid.NewGuid();

            _targetSchemaService.TargetTableExists("account").Returns(true);
            _targetSchemaService.DeleteTargetTable("account")
                .Returns(new TargetSchemaModificationResult { Success = SchemaModificationResult.Success });
            _dataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails>
                {
                    new pg_synchronizationdetails(firstId),
                    new pg_synchronizationdetails(secondId)
                });

            var result = _service.PurgeTable("account");

            Assert.True(result.IsPurged);
            Assert.Equal("account", result.TableName);
            _targetSchemaService.Received(1).DeleteTargetTable("account");
            _dataRepository.Received(1).DeleteRecord(firstId, pg_synchronizationdetails.EntityLogicalName);
            _dataRepository.Received(1).DeleteRecord(secondId, pg_synchronizationdetails.EntityLogicalName);
        }

        [Fact]
        public void PurgeTable_TargetTableDoesNotExist_DeletesSyncDetailsAndReturnsSuccess()
        {
            var syncDetailId = Guid.NewGuid();

            _targetSchemaService.TargetTableExists("account").Returns(false);
            _dataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Returns(new List<pg_synchronizationdetails>
                {
                    new pg_synchronizationdetails(syncDetailId)
                });

            var result = _service.PurgeTable("account");

            Assert.True(result.IsPurged);
            _targetSchemaService.DidNotReceive().DeleteTargetTable(Arg.Any<string>());
            _dataRepository.Received(1).DeleteRecord(syncDetailId, pg_synchronizationdetails.EntityLogicalName);
        }

        [Fact]
        public void PurgeTable_TargetDeleteFails_ReturnsFailureAndSkipsSyncDetailsDeletion()
        {
            _targetSchemaService.TargetTableExists("account").Returns(true);
            _targetSchemaService.DeleteTargetTable("account")
                .Returns(new TargetSchemaModificationResult
                {
                    Success = SchemaModificationResult.Failure,
                    Message = "Delete failed"
                });

            var result = _service.PurgeTable("account");

            Assert.False(result.IsPurged);
            Assert.Equal("Delete failed", result.ErrorMessage);
            _dataRepository.DidNotReceive().GetSyncDetails(Arg.Any<string>(), Arg.Any<pg_TargetRepositorySet>());
            _dataRepository.DidNotReceiveWithAnyArgs().DeleteRecord(default, default!);
        }

        [Fact]
        public void PurgeTable_GetSyncDetailsThrows_ReturnsFailure()
        {
            _targetSchemaService.TargetTableExists("account").Returns(false);
            _dataRepository.GetSyncDetails("account", pg_TargetRepositorySet.MicrosoftSqlServer)
                .Throws(new Exception("Dataverse unavailable"));

            var result = _service.PurgeTable("account");

            Assert.False(result.IsPurged);
            Assert.Equal("Dataverse unavailable", result.ErrorMessage);
            _dataRepository.DidNotReceiveWithAnyArgs().DeleteRecord(default, default!);
        }
    }
}
