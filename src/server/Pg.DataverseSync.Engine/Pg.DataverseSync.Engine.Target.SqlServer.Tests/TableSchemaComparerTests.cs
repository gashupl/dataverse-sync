using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer.Tests
{
    public class TableSchemaComparerTests
    {
        private readonly ILogger _logger;
        private readonly TableSchemaComparer _comparer;

        public TableSchemaComparerTests()
        {
            _logger = Substitute.For<ILogger>();
            _comparer = new TableSchemaComparer(_logger);
        }

        [Fact]
        public void GetColumnsToBeRemoved_ReturnsColumns_NotPresentInSourceTable()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", "NVARCHAR(MAX)"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)"),
                new Column("ObsoleteColumn", "INT")
            });

            var result = _comparer.GetColumnsToBeRemoved(sourceTable, targetTable);

            var removedColumn = Assert.Single(result);
            Assert.Equal("ObsoleteColumn", removedColumn.Name);
        }

        [Fact]
        public void GetColumnsToBeRemoved_ReturnsEmptyList_WhenAllColumnsMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", "NVARCHAR(MAX)"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = _comparer.GetColumnsToBeRemoved(sourceTable, targetTable);

            Assert.Empty(result);
        }

        [Fact]
        public void GetColumnsToBeAdded_ReturnsColumns_MissingFromTargetTable()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", "NVARCHAR(MAX)"));
            sourceTable.Columns.Add(new Column("NewColumn", "INT"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = _comparer.GetColumnsToBeAdded(sourceTable, targetTable);

            var addedColumn = Assert.Single(result);
            Assert.Equal("NewColumn", addedColumn.Name);
        }

        [Fact]
        public void GetColumnsToBeAdded_ReturnsEmptyList_WhenAllColumnsMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", "NVARCHAR(MAX)"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = _comparer.GetColumnsToBeAdded(sourceTable, targetTable);

            Assert.Empty(result);
        }

        [Fact]
        public void GetModifiedColumns_ReturnsMatchingSourceAndTargetColumns_WhenDataTypeDiffers()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Amount", "MONEY"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Amount", "FLOAT")
            });

            var (sourceChanges, targetChanges) = _comparer.GetModifiedColumns(sourceTable, targetTable);

            var sourceChange = Assert.Single(sourceChanges);
            var targetChange = Assert.Single(targetChanges);
            Assert.Equal("Amount", sourceChange.Name);
            Assert.Equal("MONEY", sourceChange.DataType);
            Assert.Equal("Amount", targetChange.Name);
            Assert.Equal("FLOAT", targetChange.DataType);
        }

        [Fact]
        public void GetModifiedColumns_IsCaseInsensitive_WhenMatchingColumnNames()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("amount", "MONEY"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("AMOUNT", "FLOAT")
            });

            var (sourceChanges, targetChanges) = _comparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Single(sourceChanges);
            Assert.Single(targetChanges);
        }

        [Fact]
        public void GetModifiedColumns_ReturnsEmptyLists_WhenDataTypesMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Amount", "MONEY"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Amount", "MONEY")
            });

            var (sourceChanges, targetChanges) = _comparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Empty(sourceChanges);
            Assert.Empty(targetChanges);
        }

        [Fact]
        public void GetModifiedColumns_ReturnsEmptyLists_WhenColumnDoesNotExistInTarget()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("NewColumn", "MONEY"));

            var targetTable = new SqlTable("account", new List<Column>());

            var (sourceChanges, targetChanges) = _comparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Empty(sourceChanges);
            Assert.Empty(targetChanges);
        }

        [Fact]
        public void MergeColumns_CombinesBothLists_WhenNoDuplicatesExist()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("Amount", "MONEY") };

            var result = TableSchemaComparer.MergeColumns(columnsTable1, columnsTable2);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Name");
            Assert.Contains(result, c => c.Name == "Amount");
        }

        [Fact]
        public void MergeColumns_ExcludesDuplicates_WhenColumnNamesMatchCaseInsensitively()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("name", "NVARCHAR(MAX)") };

            var result = TableSchemaComparer.MergeColumns(columnsTable1, columnsTable2);

            Assert.Single(result);
        }

        [Fact]
        public void MergeColumns_ReturnsFirstList_WhenSecondListIsEmpty()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column>();

            var result = TableSchemaComparer.MergeColumns(columnsTable1, columnsTable2);

            var mergedColumn = Assert.Single(result);
            Assert.Equal("Name", mergedColumn.Name);
        }
    }
}
