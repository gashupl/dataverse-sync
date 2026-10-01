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
        public void CreateForInsert_DataverseSdkWrapperValues_ConvertsToSqlFriendlyValues()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var parentCustomerId = Guid.NewGuid();
            var entity = new Entity("contact")
            {
                ["statecode"] = new OptionSetValue(0),
                ["creditlimit"] = new Money(125.50m),
                ["parentcustomerid"] = new EntityReference("account", parentCustomerId),
                ["donotbulkemail"] = new BooleanManagedProperty(true),
                ["preferredcontactmethodcode"] = new AliasedValue("contact", "preferredcontactmethodcode", new OptionSetValue(2)),
                ["samplemultiselect"] = new OptionSetValueCollection
                {
                    new OptionSetValue(3),
                    new OptionSetValue(7),
                },
            };

            // Act
            var result = factory.CreateForInsert(entity);

            // Assert
            Assert.Contains(result.Columns, c => c.ColumnName == "statecode" && Equals(c.Value, 0));
            Assert.Contains(result.Columns, c => c.ColumnName == "creditlimit" && Equals(c.Value, 125.50m));
            Assert.Contains(result.Columns, c => c.ColumnName == "parentcustomerid" && Equals(c.Value, parentCustomerId));
            Assert.Contains(result.Columns, c => c.ColumnName == "donotbulkemail" && Equals(c.Value, true));
            Assert.Contains(result.Columns, c => c.ColumnName == "preferredcontactmethodcode" && Equals(c.Value, 2));
            Assert.Contains(result.Columns, c => c.ColumnName == "samplemultiselect" && Equals(c.Value, "3,7"));
        }

        [Theory]
        [InlineData("/Date(1735689600000)/")]
        [InlineData("\\/Date(1735689600000)\\/")]
        [InlineData("2025-01-01T00:00:00Z")]
        public void CreateForInsert_SerializedDateTimeValue_ConvertsToDateTime(string serializedDateTime)
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var entity = new Entity("contact")
            {
                ["createdon"] = serializedDateTime,
            };

            // Act
            var result = factory.CreateForInsert(entity);

            // Assert
            var createdOnColumn = Assert.Single(result.Columns, c => c.ColumnName == "createdon");
            Assert.IsType<DateTime>(createdOnColumn.Value);
            Assert.Equal(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), (DateTime)createdOnColumn.Value);
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

            var primaryKeyColumn = Assert.Single(result.Columns, c => c.IsPrimaryKey);
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
            var primaryKeyColumn = Assert.Single(result.Columns, c => c.IsPrimaryKey);
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
        public void CreateForUpdate_DataverseSdkWrapperValues_ConvertsToSqlFriendlyValues()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var accountId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                ["accountid"] = accountId,
                ["ownerid"] = new EntityReference("systemuser", ownerId),
                ["customertypecode"] = new OptionSetValue(1),
            };

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            Assert.Contains(result.Columns, c => c.ColumnName == "accountid" && Equals(c.Value, accountId) && c.IsPrimaryKey);
            Assert.Contains(result.Columns, c => c.ColumnName == "ownerid" && Equals(c.Value, ownerId) && c.IsPrimaryKey == false);
            Assert.Contains(result.Columns, c => c.ColumnName == "customertypecode" && Equals(c.Value, 1) && c.IsPrimaryKey == false);
        }

        [Fact]
        public void CreateForUpdate_DateTimeOffsetValue_ConvertsToUtcDateTime()
        {
            // Arrange
            var factory = new TargetRecordFactory();
            var accountId = Guid.NewGuid();
            var modifiedOn = new DateTimeOffset(2025, 1, 1, 1, 30, 0, TimeSpan.FromHours(1));
            var entity = new Entity("account")
            {
                ["accountid"] = accountId,
                ["modifiedon"] = modifiedOn,
            };

            // Act
            var result = factory.CreateForUpdate(entity);

            // Assert
            var modifiedOnColumn = Assert.Single(result.Columns, c => c.ColumnName == "modifiedon");
            Assert.IsType<DateTime>(modifiedOnColumn.Value);
            Assert.Equal(new DateTime(2025, 1, 1, 0, 30, 0, DateTimeKind.Utc), (DateTime)modifiedOnColumn.Value);
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
