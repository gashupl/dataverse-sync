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
            table.Columns.Add(new Column("Id", "INT", isPrimaryKey: false, isNullable: false));
            table.Columns.Add(new Column("Name", "NVARCHAR(100)", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Customers (Id INT NOT NULL, Name NVARCHAR(100))", result);
        }

        [Fact]
        public void Generate_TableWithPrimaryKey_ReturnsCreateTableWithPrimaryKeyConstraint()
        {
            // Arrange
            var table = new Table("Products", "Products", false);
            table.Columns.Add(new Column("ProductId", "INT", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("ProductName", "NVARCHAR(200)", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Products (ProductId INT NOT NULL, ProductName NVARCHAR(200) NOT NULL, PRIMARY KEY (ProductId))", result);
        }

        [Fact]
        public void Generate_TableWithIdentityColumn_ReturnsCreateTableWithIdentity()
        {
            // Arrange
            var table = new Table("Orders", "Orders", false);
            table.Columns.Add(new Column("OrderId", "INT", isPrimaryKey: true, isIdentity: true, isNullable: false));
            table.Columns.Add(new Column("OrderDate", "DATETIME2", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Orders (OrderId INT IDENTITY(1,1) NOT NULL, OrderDate DATETIME2 NOT NULL, PRIMARY KEY (OrderId))", result);
        }

        [Fact]
        public void Generate_TableWithSingleColumn_ReturnsValidCreateTableStatement()
        {
            // Arrange
            var table = new Table("Settings", "Settings", false);
            table.Columns.Add(new Column("SettingKey", "NVARCHAR(50)", isPrimaryKey: true, isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Settings (SettingKey NVARCHAR(50) NOT NULL, PRIMARY KEY (SettingKey))", result);
        }

        [Fact]
        public void Generate_TableWithMultipleColumns_ReturnsCreateTableWithAllColumns()
        {
            // Arrange
            var table = new Table("Employees", "Employees", false);
            table.Columns.Add(new Column("EmployeeId", "UNIQUEIDENTIFIER", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("FirstName", "NVARCHAR(50)", isNullable: false));
            table.Columns.Add(new Column("LastName", "NVARCHAR(50)", isNullable: false));
            table.Columns.Add(new Column("Email", "NVARCHAR(100)", isNullable: true));
            table.Columns.Add(new Column("HireDate", "DATETIME2", isNullable: false));
            table.Columns.Add(new Column("Salary", "DECIMAL(18,2)", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            var expected = "CREATE TABLE Employees (" +
                "EmployeeId UNIQUEIDENTIFIER NOT NULL, " +
                "FirstName NVARCHAR(50) NOT NULL, " +
                "LastName NVARCHAR(50) NOT NULL, " +
                "Email NVARCHAR(100), " +
                "HireDate DATETIME2 NOT NULL, " +
                "Salary DECIMAL(18,2), " +
                "PRIMARY KEY (EmployeeId))";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Generate_TableWithAllNullableColumns_ReturnsCreateTableWithoutNotNull()
        {
            // Arrange
            var table = new Table("Logs", "Logs", false);
            table.Columns.Add(new Column("LogId", "INT", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("Message", "NVARCHAR(MAX)", isNullable: true));
            table.Columns.Add(new Column("Timestamp", "DATETIME2", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Logs (LogId INT NOT NULL, Message NVARCHAR(MAX), Timestamp DATETIME2, PRIMARY KEY (LogId))", result);
        }

        [Fact]
        public void Generate_TableWithAllNotNullableColumns_ReturnsCreateTableWithNotNull()
        {
            // Arrange
            var table = new Table("Configurations", "Configurations", false);
            table.Columns.Add(new Column("ConfigId", "INT", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("ConfigKey", "NVARCHAR(100)", isNullable: false));
            table.Columns.Add(new Column("ConfigValue", "NVARCHAR(500)", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Configurations (ConfigId INT NOT NULL, ConfigKey NVARCHAR(100) NOT NULL, ConfigValue NVARCHAR(500) NOT NULL, PRIMARY KEY (ConfigId))", result);
        }

        [Fact]
        public void Generate_TableWithDifferentDataTypes_ReturnsCreateTableWithCorrectTypes()
        {
            // Arrange
            var table = new Table("DataTypes", "DataTypes", false);
            table.Columns.Add(new Column("Id", "INT", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("StringCol", "NVARCHAR(MAX)", isNullable: true));
            table.Columns.Add(new Column("IntCol", "INT", isNullable: true));
            table.Columns.Add(new Column("BigIntCol", "BIGINT", isNullable: true));
            table.Columns.Add(new Column("DecimalCol", "DECIMAL(18,2)", isNullable: true));
            table.Columns.Add(new Column("FloatCol", "FLOAT", isNullable: true));
            table.Columns.Add(new Column("BitCol", "BIT", isNullable: true));
            table.Columns.Add(new Column("DateTimeCol", "DATETIME2", isNullable: true));
            table.Columns.Add(new Column("UniqueIdCol", "UNIQUEIDENTIFIER", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            var expected = "CREATE TABLE DataTypes (" +
                "Id INT NOT NULL, " +
                "StringCol NVARCHAR(MAX), " +
                "IntCol INT, " +
                "BigIntCol BIGINT, " +
                "DecimalCol DECIMAL(18,2), " +
                "FloatCol FLOAT, " +
                "BitCol BIT, " +
                "DateTimeCol DATETIME2, " +
                "UniqueIdCol UNIQUEIDENTIFIER, " +
                "PRIMARY KEY (Id))";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Generate_TableWithGuidPrimaryKey_ReturnsCreateTableWithUniqueIdentifierPrimaryKey()
        {
            // Arrange
            var table = new Table("Accounts", "Accounts", false);
            table.Columns.Add(new Column("AccountId", "UNIQUEIDENTIFIER", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("AccountName", "NVARCHAR(100)", isNullable: false));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Accounts (AccountId UNIQUEIDENTIFIER NOT NULL, AccountName NVARCHAR(100) NOT NULL, PRIMARY KEY (AccountId))", result);
        }

        [Fact]
        public void Generate_TableWithIdentityAndPrimaryKey_ReturnsCreateTableWithIdentityAndPrimaryKey()
        {
            // Arrange
            var table = new Table("Categories", "Categories", false);
            table.Columns.Add(new Column("CategoryId", "INT", isPrimaryKey: true, isIdentity: true, isNullable: false));
            table.Columns.Add(new Column("CategoryName", "NVARCHAR(100)", isNullable: false));
            table.Columns.Add(new Column("Description", "NVARCHAR(MAX)", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE Categories (CategoryId INT IDENTITY(1,1) NOT NULL, CategoryName NVARCHAR(100) NOT NULL, Description NVARCHAR(MAX), PRIMARY KEY (CategoryId))", result);
        }

        [Fact]
        public void Generate_TableNameWithSpecialCharacters_ReturnsCreateTableWithTableName()
        {
            // Arrange
            var table = new Table("dbo.MyTable", "My Table", false);
            table.Columns.Add(new Column("Id", "INT", isPrimaryKey: true, isNullable: false));

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
            table.Columns.Add(new Column("Column_1", "INT", isPrimaryKey: true, isNullable: false));
            table.Columns.Add(new Column("Column Name 2", "NVARCHAR(50)", isNullable: true));

            // Act
            var result = CreateTableQueryGenerator.Generate(table);

            // Assert
            Assert.Equal("CREATE TABLE TestTable (Column_1 INT NOT NULL, Column Name 2 NVARCHAR(50), PRIMARY KEY (Column_1))", result);
        }
    }
}
