using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Target.SqlServer.Tests
{
    public class TableSchemaComparerTests
    {

        [Fact]
        public void GetColumnsToBeRemoved_ReturnsColumns_NotPresentInSourceTable()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", DataverseDataTypes.String));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)"),
                new Column("ObsoleteColumn", "INT")
            });

            var result = TableSchemaComparer.GetColumnsToBeRemoved(sourceTable, targetTable);

            var removedColumn = Assert.Single(result);
            Assert.Equal("ObsoleteColumn", removedColumn.Name);
        }

        [Fact]
        public void GetColumnsToBeRemoved_ReturnsEmptyList_WhenAllColumnsMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", DataverseDataTypes.String));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = TableSchemaComparer.GetColumnsToBeRemoved(sourceTable, targetTable);

            Assert.Empty(result);
        }

        [Fact]
        public void GetColumnsToBeAdded_ReturnsColumns_MissingFromTargetTable()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", DataverseDataTypes.String));
            sourceTable.Columns.Add(new Column("NewColumn", DataverseDataTypes.Integer));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = TableSchemaComparer.GetColumnsToBeAdded(sourceTable, targetTable);

            var addedColumn = Assert.Single(result);
            Assert.Equal("NewColumn", addedColumn.Name);
            Assert.Equal("INT", addedColumn.DataType);
        }

        [Fact]
        public void GetColumnsToBeAdded_ReturnsEmptyList_WhenAllColumnsMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", DataverseDataTypes.String));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Name", "NVARCHAR(MAX)")
            });

            var result = TableSchemaComparer.GetColumnsToBeAdded(sourceTable, targetTable);

            Assert.Empty(result);
        }

        [Fact]
        public void GetColumnsToBeAdded_ConvertsDatatypeToSqlEquivalent_WhenAddingColumns()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Name", DataverseDataTypes.String));
            sourceTable.Columns.Add(new Column("Amount", DataverseDataTypes.Money));
            sourceTable.Columns.Add(new Column("Count", DataverseDataTypes.Integer));
            sourceTable.Columns.Add(new Column("IsActive", DataverseDataTypes.Boolean));
            sourceTable.Columns.Add(new Column("CreatedOn", DataverseDataTypes.DateTime));

            var targetTable = new SqlTable("account", new List<Column>());

            var result = TableSchemaComparer.GetColumnsToBeAdded(sourceTable, targetTable);

            Assert.Equal(5, result.Count);

            // Verify data type conversions
            var nameColumn = result.FirstOrDefault(c => c.Name == "Name");
            Assert.NotNull(nameColumn);
            Assert.Equal("NVARCHAR(MAX)", nameColumn.DataType);

            var amountColumn = result.FirstOrDefault(c => c.Name == "Amount");
            Assert.NotNull(amountColumn);
            Assert.Equal("MONEY", amountColumn.DataType);

            var countColumn = result.FirstOrDefault(c => c.Name == "Count");
            Assert.NotNull(countColumn);
            Assert.Equal("INT", countColumn.DataType);

            var isActiveColumn = result.FirstOrDefault(c => c.Name == "IsActive");
            Assert.NotNull(isActiveColumn);
            Assert.Equal("BIT", isActiveColumn.DataType);

            var createdOnColumn = result.FirstOrDefault(c => c.Name == "CreatedOn");
            Assert.NotNull(createdOnColumn);
            Assert.Equal("DATETIME", createdOnColumn.DataType);
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

            var (sourceChanges, targetChanges) = TableSchemaComparer.GetModifiedColumns(sourceTable, targetTable);

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

            var (sourceChanges, targetChanges) = TableSchemaComparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Single(sourceChanges);
            Assert.Single(targetChanges);
        }

        [Fact]
        public void GetModifiedColumns_ReturnsEmptyLists_WhenDataTypesMatch()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("Amount", "MoneyType"));

            var targetTable = new SqlTable("account", new List<Column>
            {
                new Column("Amount", "MONEY")
            });

            var (sourceChanges, targetChanges) = TableSchemaComparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Empty(sourceChanges);
            Assert.Empty(targetChanges);
        }

        [Fact]
        public void GetModifiedColumns_ReturnsEmptyLists_WhenColumnDoesNotExistInTarget()
        {
            var sourceTable = new Table("account", "Account", false);
            sourceTable.Columns.Add(new Column("NewColumn", "MoneyType"));

            var targetTable = new SqlTable("account", new List<Column>());

            var (sourceChanges, targetChanges) = TableSchemaComparer.GetModifiedColumns(sourceTable, targetTable);

            Assert.Empty(sourceChanges);
            Assert.Empty(targetChanges);
        }

        [Fact]
        public void MergeColumnsToBeRemoved_CombinesBothLists_WhenNoDuplicatesExist()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("Amount", "MONEY") };

            var result = TableSchemaComparer.MergeColumnsToBeRemoved(columnsTable1, columnsTable2);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Name");
            Assert.Contains(result, c => c.Name == "Amount");
        }

        [Fact]
        public void MergeColumnsToBeRemoved_ExcludesDuplicates_WhenColumnNamesMatchCaseInsensitively()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("name", "NVARCHAR(MAX)") };

            var result = TableSchemaComparer.MergeColumnsToBeRemoved(columnsTable1, columnsTable2);

            Assert.Single(result);
        }

        [Fact]
        public void MergeColumnsToBeRemoved_ReturnsFirstList_WhenSecondListIsEmpty()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column>();

            var result = TableSchemaComparer.MergeColumnsToBeRemoved(columnsTable1, columnsTable2);

            var mergedColumn = Assert.Single(result);
            Assert.Equal("Name", mergedColumn.Name);
        }

        [Fact]
        public void MergeColumnsToBeAdded_CombinesBothLists_WhenNoDuplicatesExist()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("Amount", "MONEY") };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Name");
            Assert.Contains(result, c => c.Name == "Amount");
        }

        [Fact]
        public void MergeColumnsToBeAdded_ExcludesDuplicates_WhenColumnNamesMatchCaseInsensitively()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column> { new Column("name", "NVARCHAR(100)") };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Single(result);
            Assert.Equal("Name", result[0].Name);
        }

        [Fact]
        public void MergeColumnsToBeAdded_ReturnsFirstList_WhenSecondListIsEmpty()
        {
            var columnsTable1 = new List<Column> { new Column("Name", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column>();

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            var mergedColumn = Assert.Single(result);
            Assert.Equal("Name", mergedColumn.Name);
        }

        [Fact]
        public void MergeColumnsToBeAdded_ReturnsSecondList_WhenFirstListIsEmpty()
        {
            var columnsTable1 = new List<Column>();
            var columnsTable2 = new List<Column> { new Column("Amount", "MONEY") };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            var mergedColumn = Assert.Single(result);
            Assert.Equal("Amount", mergedColumn.Name);
        }

        [Fact]
        public void MergeColumnsToBeAdded_NormalizesDataType_ForColumnsFromSecondList()
        {
            var columnsTable1 = new List<Column>();
            var columnsTable2 = new List<Column> { new Column("Status", DataverseDataTypes.Status) };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            var mergedColumn = Assert.Single(result);
            Assert.Equal("Status", mergedColumn.Name);
            Assert.Equal("INT", mergedColumn.DataType);
        }

        [Fact]
        public void MergeColumnsToBeAdded_CombinesMultipleColumns_WithoutDuplicates()
        {
            var columnsTable1 = new List<Column>
            {
                new Column("Id", "INT"),
                new Column("Name", "NVARCHAR(MAX)"),
                new Column("Email", "NVARCHAR(255)")
            };
            var columnsTable2 = new List<Column>
            {
                new Column("Email", "NVARCHAR(255)"),
                new Column("Amount", "MONEY"),
                new Column("IsActive", "BIT")
            };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(5, result.Count);
            Assert.Contains(result, c => c.Name == "Id");
            Assert.Contains(result, c => c.Name == "Name");
            Assert.Contains(result, c => c.Name == "Email");
            Assert.Contains(result, c => c.Name == "Amount");
            Assert.Contains(result, c => c.Name == "IsActive");
        }

        [Fact]
        public void MergeColumnsToBeAdded_HandlesMixedCaseNames_CaseInsensitively()
        {
            var columnsTable1 = new List<Column> { new Column("ProductName", "NVARCHAR(MAX)") };
            var columnsTable2 = new List<Column>
            {
                new Column("productname", "NVARCHAR(100)"),
                new Column("PRODUCTID", "INT")
            };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(2, result.Count);
            Assert.Single(result.Where(c => c.Name!.Equals("ProductName", StringComparison.OrdinalIgnoreCase)));
            Assert.Single(result.Where(c => c.Name!.Equals("PRODUCTID", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void MergeColumnsToBeAdded_PresservesColumnProperties_AndNormalizesDataType()
        {
            var columnsTable1 = new List<Column>();
            var columnsTable2 = new List<Column>
            {
                new Column("Id", DataverseDataTypes.Uniqueidentifier, isPrimaryKey: true, isIdentity: true, isNullable: false),
                new Column("Name", DataverseDataTypes.String, isPrimaryKey: false, isIdentity: false, isNullable: true)
            };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(2, result.Count);

            var idColumn = result.FirstOrDefault(c => c.Name == "Id");
            Assert.NotNull(idColumn);
            Assert.Equal("UNIQUEIDENTIFIER", idColumn.DataType);
            Assert.True(idColumn.IsPrimaryKey);
            Assert.True(idColumn.IsIdentity);
            Assert.False(idColumn.IsNullable);

            var nameColumn = result.FirstOrDefault(c => c.Name == "Name");
            Assert.NotNull(nameColumn);
            Assert.Equal("NVARCHAR(MAX)", nameColumn.DataType);
            Assert.False(nameColumn.IsPrimaryKey);
            Assert.False(nameColumn.IsIdentity);
            Assert.True(nameColumn.IsNullable);
        }

        [Fact]
        public void MergeColumnsToBeAdded_ReturnsAllColumns_WhenBothListsHaveSameSize()
        {
            var columnsTable1 = new List<Column>
            {
                new Column("Column1", "INT"),
                new Column("Column2", "NVARCHAR(MAX)")
            };
            var columnsTable2 = new List<Column>
            {
                new Column("Column3", "bit"),
                new Column("Column4", "money")
            };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(4, result.Count);
            Assert.Equal(new[] { "Column1", "Column2", "Column3", "Column4" },
                result.Select(c => c.Name).ToList());
        }

        [Fact]
        public void MergeColumnsToBeAdded_ReturnsEmptyList_WhenBothListsAreEmpty()
        {
            var columnsTable1 = new List<Column>();
            var columnsTable2 = new List<Column>();

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Empty(result);
        }

        [Fact]
        public void MergeColumnsToBeAdded_NormalizesMultipleDataTypes_FromSecondList()
        {
            var columnsTable1 = new List<Column>();
            var columnsTable2 = new List<Column>
            {
                new Column("StringCol", DataverseDataTypes.String),
                new Column("IntCol", DataverseDataTypes.Integer),
                new Column("BoolCol", DataverseDataTypes.Boolean),
                new Column("MoneyCol", DataverseDataTypes.Money),
                new Column("DateCol", DataverseDataTypes.DateTime)
            };

            var result = TableSchemaComparer.MergeColumnsToBeAdded(columnsTable1, columnsTable2);

            Assert.Equal(5, result.Count);
            Assert.Equal("NVARCHAR(MAX)", result.First(c => c.Name == "StringCol").DataType);
            Assert.Equal("INT", result.First(c => c.Name == "IntCol").DataType);
            Assert.Equal("BIT", result.First(c => c.Name == "BoolCol").DataType);
            Assert.Equal("MONEY", result.First(c => c.Name == "MoneyCol").DataType);
            Assert.Equal("DATETIME", result.First(c => c.Name == "DateCol").DataType);
        }

    }
}
