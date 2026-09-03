using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer.Tests
{
    public class CreateTableQueryGeneratorTests
    {
        [Fact]
        public void Generate_SimpleTableWithoutPrimaryKey_ReturnsValidCreateTableStatement()
        {
            // Arrange
            var table = new Table("Customers", "Customers", false);
            table.Columns.Add(new Column("Id", "IntegerType", isPrimaryKey: false, isNullable: false));
            table.Columns.Add(new Column("Name", "StringType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Customers (Id INT NOT NULL, Name NVARCHAR(MAX))", result);
        }

        [Fact]
        public void Generate_TableWithPrimaryKey_ReturnsCreateTableWithPrimaryKeyConstraint()
        {
            // Arrange
            var table = new Table("Products", "Products", false);
            table.Columns.Add(new Column("ProductId", "IntegerType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("ProductName", "StringType", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Products (ProductId INT NOT NULL, ProductName NVARCHAR(MAX) NOT NULL, PRIMARY KEY (ProductId))", result);
        }

        [Fact]
        public void Generate_TableWithIdentityColumn_ReturnsCreateTableWithIdentity()
        {
            // Arrange
            var table = new Table("Orders", "Orders", false);
            table.Columns.Add(new Column("OrderId", "IntegerType", isPrimaryKey: true, isIdentity: true, isNullable: false));
            table.Columns.Add(new Column("OrderDate", "DateTimeType", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Orders (OrderId INT IDENTITY(1,1) NOT NULL, OrderDate DATETIME NOT NULL, PRIMARY KEY (OrderId))", result);
        }

        [Fact]
        public void Generate_TableWithSingleColumn_ReturnsValidCreateTableStatement()
        {
            // Arrange
            var table = new Table("Settings", "Settings", false);
            table.Columns.Add(new Column("SettingKey", "StringType", isPrimaryKey: true, isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Settings (SettingKey NVARCHAR(MAX) NOT NULL, PRIMARY KEY (SettingKey))", result);
        }

        [Fact]
        public void Generate_TableWithMultipleColumns_ReturnsCreateTableWithAllColumns()
        {
            // Arrange
            var table = new Table("Employees", "Employees", false);
            table.Columns.Add(new Column("EmployeeId", "UniqueidentifierType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("FirstName", "StringType", isNullable: false));
            table.Columns.Add(new Column("LastName", "StringType", isNullable: false));
            table.Columns.Add(new Column("Email", "StringType", isNullable: true));
            table.Columns.Add(new Column("HireDate", "DateTimeType", isNullable: false));
            table.Columns.Add(new Column("Salary", "DecimalType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            var expected = "CREATE TABLE Employees (" +
                "EmployeeId UNIQUEIDENTIFIER NOT NULL, " +
                "FirstName NVARCHAR(MAX) NOT NULL, " +
                "LastName NVARCHAR(MAX) NOT NULL, " +
                "Email NVARCHAR(MAX), " +
                "HireDate DATETIME NOT NULL, " +
                "Salary DECIMAL(38,0), " +
                "PRIMARY KEY (EmployeeId))";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Generate_TableWithAllNullableColumns_ReturnsCreateTableWithoutNotNull()
        {
            // Arrange
            var table = new Table("Logs", "Logs", false);
            table.Columns.Add(new Column("LogId", "IntegerType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("Message", "StringType", isNullable: true));
            table.Columns.Add(new Column("Timestamp", "DateTimeType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Logs (LogId INT NOT NULL, Message NVARCHAR(MAX), Timestamp DATETIME, PRIMARY KEY (LogId))", result);
        }

        [Fact]
        public void Generate_TableWithAllNotNullableColumns_ReturnsCreateTableWithNotNull()
        {
            // Arrange
            var table = new Table("Configurations", "Configurations", false);
            table.Columns.Add(new Column("ConfigId", "IntegerType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("ConfigKey", "StringType", isNullable: false));
            table.Columns.Add(new Column("ConfigValue", "StringType", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Configurations (ConfigId INT NOT NULL, ConfigKey NVARCHAR(MAX) NOT NULL, ConfigValue NVARCHAR(MAX) NOT NULL, PRIMARY KEY (ConfigId))", result);
        }

        [Fact]
        public void Generate_TableWithDifferentDataTypes_ReturnsCreateTableWithCorrectTypes()
        {
            // Arrange
            var table = new Table("DataTypes", "DataTypes", false);
            table.Columns.Add(new Column("Id", "IntegerType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("StringCol", "StringType", isNullable: true));
            table.Columns.Add(new Column("IntCol", "IntegerType", isNullable: true));
            table.Columns.Add(new Column("BigIntCol", "BigIntType", isNullable: true));
            table.Columns.Add(new Column("DecimalCol", "DecimalType", isNullable: true));
            table.Columns.Add(new Column("FloatCol", "DoubleType", isNullable: true));
            table.Columns.Add(new Column("BitCol", "BooleanType", isNullable: true));
            table.Columns.Add(new Column("DateTimeCol", "DateTimeType", isNullable: true));
            table.Columns.Add(new Column("UniqueIdCol", "UniqueidentifierType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            var expected = "CREATE TABLE DataTypes (" +
                "Id INT NOT NULL, " +
                "StringCol NVARCHAR(MAX), " +
                "IntCol INT, " +
                "BigIntCol BIGINT, " +
                "DecimalCol DECIMAL(38,0), " +
                "FloatCol FLOAT, " +
                "BitCol BIT, " +
                "DateTimeCol DATETIME, " +
                "UniqueIdCol UNIQUEIDENTIFIER, " +
                "PRIMARY KEY (Id))";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Generate_TableWithGuidPrimaryKey_ReturnsCreateTableWithUniqueIdentifierPrimaryKey()
        {
            // Arrange
            var table = new Table("Accounts", "Accounts", false);
            table.Columns.Add(new Column("AccountId", "UniqueidentifierType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("AccountName", "StringType", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Accounts (AccountId UNIQUEIDENTIFIER NOT NULL, AccountName NVARCHAR(MAX) NOT NULL, PRIMARY KEY (AccountId))", result);
        }

        [Fact]
        public void Generate_TableWithIdentityAndPrimaryKey_ReturnsCreateTableWithIdentityAndPrimaryKey()
        {
            // Arrange
            var table = new Table("Categories", "Categories", false);
            table.Columns.Add(new Column("CategoryId", "IntegerType", isPrimaryKey: true, isIdentity: true, isNullable: false));
            table.Columns.Add(new Column("CategoryName", "StringType", isNullable: false));
            table.Columns.Add(new Column("Description", "StringType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Categories (CategoryId INT IDENTITY(1,1) NOT NULL, CategoryName NVARCHAR(MAX) NOT NULL, Description NVARCHAR(MAX), PRIMARY KEY (CategoryId))", result);
        }

        [Fact]
        public void Generate_TableNameWithSpecialCharacters_ReturnsCreateTableWithTableName()
        {
            // Arrange
            var table = new Table("dbo.MyTable", "My Table", false);
            table.Columns.Add(new Column("Id", "IntegerType", isPrimaryKey: true, isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE dbo.MyTable (Id INT NOT NULL, PRIMARY KEY (Id))", result);
        }

        [Fact]
        public void Generate_ColumnNameWithSpecialCharacters_ReturnsCreateTableWithColumnName()
        {
            // Arrange
            var table = new Table("TestTable", "Test Table", false);
            table.Columns.Add(new Column("Column_1", "IntegerType", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("Column Name 2", "StringType", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE TestTable (Column_1 INT NOT NULL, Column Name 2 NVARCHAR(MAX), PRIMARY KEY (Column_1))", result);
        }
    }
}
