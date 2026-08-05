using Pg.DataverseSync.Engine.Target.SqlServer;

namespace Pg.DataverseSync.Engine.Target.SqlServer.Tests
{
    public class DataTypesConverterTests
    {
        [Theory]
        [InlineData("BooleanType", "BIT")]
        [InlineData("CustomerType", "UNIQUEIDENTIFIER")]
        [InlineData("DateTimeType", "DATETIME")]
        [InlineData("DecimalType", "DECIMAL(38,0)")]
        [InlineData("DoubleType", "FLOAT")]
        [InlineData("IntegerType", "INT")]
        [InlineData("LookupType", "UNIQUEIDENTIFIER")]
        [InlineData("MemoType", "NTEXT")]
        [InlineData("MoneyType", "MONEY")]
        [InlineData("OwnerType", "UNIQUEIDENTIFIER")]
        [InlineData("PicklistType", "INT")]
        [InlineData("StateType", "INT")]
        [InlineData("StatusType", "INT")]
        [InlineData("StringType", "NVARCHAR(MAX)")]
        [InlineData("UniqueidentifierType", "UNIQUEIDENTIFIER")]
        [InlineData("CalendarRulesType", "NVARCHAR(MAX)")]
        [InlineData("VirtualType", "NVARCHAR(MAX)")]
        [InlineData("BigIntType", "BIGINT")]
        [InlineData("ManagedPropertyType", "NVARCHAR(MAX)")]
        [InlineData("EntityNameType", "NVARCHAR(MAX)")]
        [InlineData("ImageType", "VARBINARY(MAX)")]
        [InlineData("MultiSelectPicklistType", "NVARCHAR(MAX)")]
        [InlineData("FileType", "VARBINARY(MAX)")]
        public void MapToSqlDataType_ReturnsExpectedSqlType_ForKnownDataverseType(string dataType, string expectedSqlType)
        {
            var result = DataTypesConverter.MapToSqlDataType(dataType);

            Assert.Equal(expectedSqlType, result);
        }

        [Fact]
        public void MapToSqlDataType_ReturnsNull_ForCustomType()
        {
            var result = DataTypesConverter.MapToSqlDataType("CustomType");

            Assert.Null(result);
        }

        [Fact]
        public void MapToSqlDataType_ReturnsNull_ForUnknownDataType()
        {
            var result = DataTypesConverter.MapToSqlDataType("SomeUnknownType");

            Assert.Null(result);
        }
    }
}
