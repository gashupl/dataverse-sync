using Microsoft.Xrm.Sdk;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class TargetRecordFactoryTests
    {
        [Fact]
        public void CreateForInsert_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new TargetRecordFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.CreateForInsert(null!));
        }

        [Fact]
        public void CreateForInsert_EntityWithoutAttributes_CreatesEmptyTargetRecord()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entity = new Entity("account");

            // Act
            var result = factory.CreateForInsert(entity);

            // Assert
            Assert.Equal("account", result.TableName);
            Assert.NotNull(result.Columns);
            Assert.Empty(result.Columns);
        }

        [Fact]
        public void CreateForInsert_EntityWithAttributes_CreatesTargetRecordWithAllColumns()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entity = new Entity("account")
            {
                ["name"] = "Contoso",
                ["accountnumber"] = "ACC-001",
                ["creditonhold"] = true,
            };

            // Act
            var result = factory.CreateForInsert(entity);

            // Assert
            Assert.Equal("account", result.TableName);
            Assert.Equal(3, result.Columns.Count);
            Assert.Contains(result.Columns, c => c.ColumnName == "name" && Equals(c.Value, "Contoso") && c.IsPrimaryKey == false);
            Assert.Contains(result.Columns, c => c.ColumnName == "accountnumber" && Equals(c.Value, "ACC-001") && c.IsPrimaryKey == false);
            Assert.Contains(result.Columns, c => c.ColumnName == "creditonhold" && Equals(c.Value, true) && c.IsPrimaryKey == false);
        }

        [Fact]
        public void CreateForUpdate_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new TargetRecordFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.CreateForUpdate(null!));
        }

        [Fact]
        public void CreateForUpdate_EntityWithoutAttributes_CreatesEmptyTargetRecord()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entity = new Entity("account");

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            Assert.Equal("account", result.TableName);
            Assert.NotNull(result.Columns);
            Assert.Empty(result.Columns);
        }

        [Fact]
        public void CreateForUpdate_PrimaryKeyAttribute_MarksPrimaryKeyColumn()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entityId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                ["accountid"] = entityId,
                ["name"] = "Contoso",
            };

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            Assert.Equal("account", result.TableName);
            Assert.Equal(2, result.Columns.Count);

            var primaryKeyColumn = Assert.Single(result.Columns.Where(c => c.IsPrimaryKey));
            Assert.Equal("accountid", primaryKeyColumn.ColumnName);
            Assert.Equal(entityId, primaryKeyColumn.Value);
            Assert.Contains(result.Columns, c => c.ColumnName == "name" && Equals(c.Value, "Contoso") && c.IsPrimaryKey == false);
        }

        [Fact]
        public void CreateForUpdate_PrimaryKeyAttributeComparison_IsCaseInsensitive()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entityId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                ["AccountId"] = entityId,
                ["name"] = "Contoso",
            };

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            var primaryKeyColumn = Assert.Single(result.Columns.Where(c => c.IsPrimaryKey));
            Assert.Equal("AccountId", primaryKeyColumn.ColumnName);
            Assert.Equal(entityId, primaryKeyColumn.Value);
        }

        [Fact]
        public void CreateForUpdate_NonPrimaryKeyAttributes_DoesNotMarkAsPrimaryKey()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entity = new Entity("account")
            {
                ["name"] = "Contoso",
                ["accountnumber"] = "ACC-001",
            };

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            Assert.Equal(2, result.Columns.Count);
            Assert.DoesNotContain(result.Columns, c => c.IsPrimaryKey);
        }

        [Fact]
        public void CreateForDelete_NullEntityReference_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new TargetRecordFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.CreateForDelete(null!));
        }

        [Fact]
        public void CreateForDelete_ValidEntityReference_CreatesTargetRecordWithPrimaryKeyOnly()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entityId = Guid.NewGuid();
            var entityReference = new EntityReference("account", entityId);

            // Act
            var result = factory.CreateForDelete(entityReference);

            // Assert
            Assert.Equal("account", result.TableName);
            Assert.Single(result.Columns);

            var column = result.Columns[0];
            Assert.Equal("accountid", column.ColumnName);
            Assert.Equal(entityId, column.Value);
            Assert.True(column.IsPrimaryKey);
        }
    }
}
