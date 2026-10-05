using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Target;
using System.Linq;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class DataLoadServiceTests
    {
        private static readonly string[] AccountNames = { "account" };
        private static readonly string[] AccountColumns = { "accountid", "name" };

        [Fact]
        public void LoadInitialData_ValidTable_LoadsAllRecordsAndReturnsResults()
        {
            // Arrange
            var dataRepository = Substitute.For<IDataRepository>();
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var logger = Substitute.For<ILogger<DataLoadService>>();

            var table = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new Column("accountid", "Uniqueidentifier", true),
                    new Column("name", "String")
                }
            };

            var sourceRecord1 = new Entity("account");
            sourceRecord1.Attributes.Add("accountid", Guid.NewGuid());
            sourceRecord1.Attributes.Add("name", "First account");

            var sourceRecord2 = new Entity("account");
            sourceRecord2.Attributes.Add("accountid", Guid.NewGuid());
            sourceRecord2.Attributes.Add("name", "Second account");

            var targetRecord1 = new TargetRecord("account");
            var targetRecord2 = new TargetRecord("account");

            sourceMetadataService.GetTables(Arg.Is<List<string>>(names => names.SequenceEqual(AccountNames)))
                .Returns(new List<Table> { table });
            dataRepository.GetRecords("account", Arg.Any<List<string>>(), null)
                .Returns(new List<Entity> { sourceRecord1, sourceRecord2 });
            targetRecordFactory.CreateForInsert(sourceRecord1).Returns(targetRecord1);
            targetRecordFactory.CreateForInsert(sourceRecord2).Returns(targetRecord2);
            targetDataRepository.InsertRecord(targetRecord1)
                .Returns(new TargetRecordModificationResult { Success = true });
            targetDataRepository.InsertRecord(targetRecord2)
                .Returns(new TargetRecordModificationResult { Success = false, Message = "Insert failed" });

            var service = new DataLoadService(
                dataRepository,
                sourceMetadataService,
                targetDataRepository,
                targetRecordFactory,
                logger);

            // Act
            var result = service.LoadInitialData("account");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result[0].Success);
            Assert.False(result[1].Success);
            Assert.Equal("Insert failed", result[1].Message);

            dataRepository.Received(1).GetRecords(
                "account",
                Arg.Is<List<string>>(columns => columns.SequenceEqual(AccountColumns)),
                null);
            targetDataRepository.Received(1).InsertRecord(targetRecord1);
            targetDataRepository.Received(1).InsertRecord(targetRecord2);
        }

        [Fact]
        public void LoadInitialData_InsertThrowsException_AddsFailureResultAndContinues()
        {
            // Arrange
            var dataRepository = Substitute.For<IDataRepository>();
            var sourceMetadataService = Substitute.For<ISourceMetadataService>();
            var targetDataRepository = Substitute.For<ITargetDataRepository>();
            var targetRecordFactory = Substitute.For<ITargetRecordFactory>();
            var logger = Substitute.For<ILogger<DataLoadService>>();

            var table = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new Column("accountid", "Uniqueidentifier", true),
                    new Column("name", "String")
                }
            };

            var sourceRecord1 = new Entity("account");
            sourceRecord1.Attributes.Add("accountid", Guid.NewGuid());
            sourceRecord1.Attributes.Add("name", "First account");

            var sourceRecord2 = new Entity("account");
            sourceRecord2.Attributes.Add("accountid", Guid.NewGuid());
            sourceRecord2.Attributes.Add("name", "Second account");

            var targetRecord1 = new TargetRecord("account");
            var targetRecord2 = new TargetRecord("account");

            sourceMetadataService.GetTables(Arg.Is<List<string>>(names => names.SequenceEqual(AccountNames)))
                .Returns(new List<Table> { table });
            dataRepository.GetRecords("account", Arg.Any<List<string>>(), null)
                .Returns(new List<Entity> { sourceRecord1, sourceRecord2 });
            targetRecordFactory.CreateForInsert(sourceRecord1).Returns(targetRecord1);
            targetRecordFactory.CreateForInsert(sourceRecord2).Returns(targetRecord2);
            targetDataRepository.InsertRecord(targetRecord1)
                .Throws(new InvalidOperationException("Insert failed"));
            targetDataRepository.InsertRecord(targetRecord2)
                .Returns(new TargetRecordModificationResult { Success = true });

            var service = new DataLoadService(
                dataRepository,
                sourceMetadataService,
                targetDataRepository,
                targetRecordFactory,
                logger);

            // Act
            var result = service.LoadInitialData("account");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.False(result[0].Success);
            Assert.Equal("Insert failed", result[0].Message);
            Assert.True(result[1].Success);

            targetDataRepository.Received(1).InsertRecord(targetRecord1);
            targetDataRepository.Received(1).InsertRecord(targetRecord2);
        }
    }
}
