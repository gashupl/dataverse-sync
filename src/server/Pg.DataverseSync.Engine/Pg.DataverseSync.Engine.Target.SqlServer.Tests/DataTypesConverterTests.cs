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

        [Theory]
        [InlineData("varbinary", "VARBINARY(MAX)")]
        [InlineData("VARBINARY", "VARBINARY(MAX)")]
        [InlineData("VarBinary", "VARBINARY(MAX)")]
        [InlineData("decimal", "DECIMAL(38,0)")]
        [InlineData("DECIMAL", "DECIMAL(38,0)")]
        [InlineData("Decimal", "DECIMAL(38,0)")]
        [InlineData("nvarchar", "NVARCHAR(MAX)")]
        [InlineData("NVARCHAR", "NVARCHAR(MAX)")]
        [InlineData("NVarChar", "NVARCHAR(MAX)")]
        public void NormalizeSqlDataType_NormalizesAbbreviatedTypes_ToTheirMaxEquivalents(string sqlDataType, string expectedNormalizedType)
        {
            var result = DataTypesConverter.NormalizeSqlDataType(sqlDataType);

            Assert.Equal(expectedNormalizedType, result);
        }

        [Theory]
        [InlineData("varchar")]
        [InlineData("VARCHAR")]
        [InlineData("VarChar")]
        public void NormalizeSqlDataType_LeavesUnchanged_ForVarcharWithoutConstraint(string sqlDataType)
        {
            var result = DataTypesConverter.NormalizeSqlDataType(sqlDataType);

            Assert.Equal(sqlDataType, result);
        }

        [Theory]
        [InlineData("BIGINT", "BIGINT")]
        [InlineData("INT", "INT")]
        [InlineData("BIT", "BIT")]
        [InlineData("UNIQUEIDENTIFIER", "UNIQUEIDENTIFIER")]
        [InlineData("DATETIME", "DATETIME")]
        [InlineData("DECIMAL(38,0)", "DECIMAL(38,0)")]
        [InlineData("MONEY", "MONEY")]
        [InlineData("FLOAT", "FLOAT")]
        [InlineData("NTEXT", "NTEXT")]
        public void NormalizeSqlDataType_LeavesUnchanged_ForAlreadyNormalizedTypes(string sqlDataType, string expectedResult)
        {
            var result = DataTypesConverter.NormalizeSqlDataType(sqlDataType);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NormalizeSqlDataType_ReturnsNull_WhenInputIsNull()
        {
            var result = DataTypesConverter.NormalizeSqlDataType(null!);

            Assert.Null(result);
        }

        [Fact]
        public void NormalizeSqlDataType_ReturnsEmpty_WhenInputIsEmpty()
        {
            var result = DataTypesConverter.NormalizeSqlDataType("");

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("varbinary(1)", "varbinary(1)")]
        [InlineData("varchar(50)", "varchar(50)")]
        [InlineData("nvarchar(100)", "nvarchar(100)")]
        public void NormalizeSqlDataType_LeavesUnchanged_ForTypesWithExplicitSizeConstraints(string sqlDataType, string expectedResult)
        {
            var result = DataTypesConverter.NormalizeSqlDataType(sqlDataType);

            Assert.Equal(expectedResult, result);
        }
    }
}
