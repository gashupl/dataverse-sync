namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    internal static class DataTypesConverter
    {
        internal static string? MapToSqlDataType(string dataType)
        {
            string? sqlDataType;

            switch (dataType)
            {
                case "BooleanType":
                    sqlDataType = "BIT";
                    break;
                case "CustomerType":
                    sqlDataType = "UNIQUEIDENTIFIER";
                    break;
                case "DateTimeType":
                    sqlDataType = "DATETIME";
                    break;
                case "DecimalType":
                    sqlDataType = $"DECIMAL(38,0)";
                    break;
                case "DoubleType":
                    sqlDataType = "FLOAT";
                    break;
                case "IntegerType":
                    sqlDataType = "INT";
                    break;
                case "LookupType":
                    sqlDataType = "UNIQUEIDENTIFIER";
                    break;
                case "MemoType":
                    sqlDataType = "NTEXT";
                    break;
                case "MoneyType":
                    sqlDataType = "MONEY";
                    break;
                case "OwnerType":
                    sqlDataType = "UNIQUEIDENTIFIER";
                    break;
                //case "PartyListType":
                //    sqlDataType = "NVARCHAR(MAX)";
                //    break;
                case "PicklistType":
                    sqlDataType = "INT";
                    break;
                case "StateType":
                    sqlDataType = "INT";
                    break;
                case "StatusType":
                    sqlDataType = "INT";
                    break;
                case "StringType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "UniqueidentifierType":
                    sqlDataType = "UNIQUEIDENTIFIER";
                    break;
                case "CalendarRulesType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "VirtualType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "BigIntType":
                    sqlDataType = "BIGINT";
                    break;
                case "ManagedPropertyType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "EntityNameType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "ImageType":
                    sqlDataType = "VARBINARY(MAX)";
                    break;
                case "MultiSelectPicklistType":
                    sqlDataType = "NVARCHAR(MAX)";
                    break;
                case "FileType":
                    sqlDataType = "VARBINARY(MAX)";
                    break;
                case "CustomType":
                    sqlDataType = null;
                    break;
                default:
                    sqlDataType = null;
                    break;
            }

            return sqlDataType;
        }
    }
}
