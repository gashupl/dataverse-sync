namespace Pg.DataverseSync.Engine.Target.Tests
{
    public class TargetRecordTests
    {
        [Fact]
        public void Constructor_WithTableName_InitializesTargetRecord()
        {
            // Arrange
            string tableName = "Accounts";

            // Act
            var targetRecord = new TargetRecord(tableName);

            // Assert
            Assert.Equal(tableName, targetRecord.TableName);
            Assert.NotNull(targetRecord.Columns);
            Assert.Empty(targetRecord.Columns);
        }

        [Theory]
        [InlineData("Contacts")]
        [InlineData("Leads")]
        [InlineData("Opportunities")]
        [InlineData("")]
        public void Constructor_WithVariousTableNames_SetsTableNameCorrectly(string tableName)
        {
            // Act
            var targetRecord = new TargetRecord(tableName);

            // Assert
            Assert.Equal(tableName, targetRecord.TableName);
        }

        [Fact]
        public void AddColumn_WithValidColumn_AddsColumnToList()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            string columnName = "Name";
            object columnValue = "Contoso Inc.";

            // Act
            targetRecord.AddColumn(columnName, columnValue);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(columnName, targetRecord.Columns[0].ColumnName);
            Assert.Equal(columnValue, targetRecord.Columns[0].Value);
            Assert.False(targetRecord.Columns[0].IsPrimaryKey);
        }

        [Fact]
        public void AddColumn_WithPrimaryKeyColumn_AddsPrimaryKeyColumn()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            string columnName = "AccountId";
            object columnValue = 1;

            // Act
            targetRecord.AddColumn(columnName, columnValue, isPrimaryKey: true);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(columnName, targetRecord.Columns[0].ColumnName);
            Assert.Equal(columnValue, targetRecord.Columns[0].Value);
            Assert.True(targetRecord.Columns[0].IsPrimaryKey);
        }

        [Fact]
        public void AddColumn_WithNullValue_AddsColumnWithNullValue()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            string columnName = "Description";

            // Act
            targetRecord.AddColumn(columnName, null!);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Null(targetRecord.Columns[0].Value);
        }

        [Theory]
        [InlineData("StringColumn", "TestValue")]
        [InlineData("IntColumn", 42)]
        [InlineData("DoubleColumn", 3.14)]
        [InlineData("BoolColumn", true)]
        public void AddColumn_WithVariousValueTypes_AddsColumnCorrectly(string columnName, object value)
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            targetRecord.AddColumn(columnName, value);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(value, targetRecord.Columns[0].Value);
        }

        [Fact]
        public void AddColumn_MultipleRegularColumns_AddsAllColumns()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            targetRecord.AddColumn("Name", "Contoso Inc.");
            targetRecord.AddColumn("Phone", "1234567890");
            targetRecord.AddColumn("Email", "info@contoso.com");

            // Assert
            Assert.Equal(3, targetRecord.Columns.Count);
            Assert.Equal("Name", targetRecord.Columns[0].ColumnName);
            Assert.Equal("Phone", targetRecord.Columns[1].ColumnName);
            Assert.Equal("Email", targetRecord.Columns[2].ColumnName);
        }

        [Fact]
        public void AddColumn_WithTwoPrimaryKeys_ThrowsInvalidOperationException()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            targetRecord.AddColumn("AccountId", 1, isPrimaryKey: true);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                targetRecord.AddColumn("ContactId", 2, isPrimaryKey: true);
            });

            Assert.Equal("A primary key column already exists for this record.", exception.Message);
        }

        [Fact]
        public void AddColumn_WithPrimaryKeyThenRegularColumns_AddsAllColumnsSuccessfully()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            targetRecord.AddColumn("AccountId", 1, isPrimaryKey: true);
            targetRecord.AddColumn("Name", "Contoso Inc.");
            targetRecord.AddColumn("Phone", "1234567890");

            // Assert
            Assert.Equal(3, targetRecord.Columns.Count);
            Assert.True(targetRecord.Columns[0].IsPrimaryKey);
            Assert.False(targetRecord.Columns[1].IsPrimaryKey);
            Assert.False(targetRecord.Columns[2].IsPrimaryKey);
        }

        [Fact]
        public void AddColumn_WithRegularColumnsThenPrimaryKey_AddsAllColumnsSuccessfully()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            targetRecord.AddColumn("Name", "Contoso Inc.");
            targetRecord.AddColumn("Phone", "1234567890");
            targetRecord.AddColumn("AccountId", 1, isPrimaryKey: true);

            // Assert
            Assert.Equal(3, targetRecord.Columns.Count);
            Assert.False(targetRecord.Columns[0].IsPrimaryKey);
            Assert.False(targetRecord.Columns[1].IsPrimaryKey);
            Assert.True(targetRecord.Columns[2].IsPrimaryKey);
        }

        [Fact]
        public void AddColumn_ExecutedMultipleTimes_ColumnListGrowsCorrectly()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            for (int i = 0; i < 5; i++)
            {
                targetRecord.AddColumn($"Column{i}", i);
            }

            // Assert
            Assert.Equal(5, targetRecord.Columns.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal($"Column{i}", targetRecord.Columns[i].ColumnName);
                Assert.Equal(i, targetRecord.Columns[i].Value);
            }
        }

        [Fact]
        public void AddColumn_WithGuidValue_AddsColumnCorrectly()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            var guidValue = Guid.NewGuid();

            // Act
            targetRecord.AddColumn("UniqueId", guidValue);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(guidValue, targetRecord.Columns[0].Value);
        }

        [Fact]
        public void AddColumn_WithDateTimeValue_AddsColumnCorrectly()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            var dateTimeValue = DateTime.Now;

            // Act
            targetRecord.AddColumn("CreatedOn", dateTimeValue);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(dateTimeValue, targetRecord.Columns[0].Value);
        }

        [Fact]
        public void AddColumn_WithComplexObject_AddsColumnCorrectly()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");
            var complexObject = new { Id = 1, Name = "Test" };

            // Act
            targetRecord.AddColumn("ComplexData", complexObject);

            // Assert
            Assert.Single(targetRecord.Columns);
            Assert.Equal(complexObject, targetRecord.Columns[0].Value);
        }

        [Fact]
        public void AddColumn_DuplicateColumnName_ThrowsInvalidOperationException()
        {
            // Arrange
            var targetRecord = new TargetRecord("Accounts");

            // Act
            targetRecord.AddColumn("Name", "Contoso");

            // Assert
            Assert.Throws<InvalidOperationException>(() => targetRecord.AddColumn("Name", "Acme"));
        }   
    }
}
