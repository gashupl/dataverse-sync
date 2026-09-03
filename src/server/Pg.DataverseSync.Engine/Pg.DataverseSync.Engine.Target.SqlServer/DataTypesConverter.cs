using Pg.DataverseSync.Engine.Core.Schema;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    internal static class DataTypesConverter
    {
        internal static string? MapToSqlDataType(string? dataType)
        {
            string? sqlDataType;

            switch (dataType)
            {
                case DataverseDataTypes.Boolean:
                    sqlDataType = SqlDataTypes.Bit; 
                    break;
                case DataverseDataTypes.Customer:
                    sqlDataType = SqlDataTypes.UniqueIdentifier;
                    break;
                case DataverseDataTypes.DateTime:
                    sqlDataType = SqlDataTypes.DateTime;
                    break;
                case DataverseDataTypes.Decimal:
                    sqlDataType = SqlDataTypes.Decimal;
                    break;
                case DataverseDataTypes.Double:
                    sqlDataType = SqlDataTypes.Float;
                    break;
                case DataverseDataTypes.Integer:
                    sqlDataType = SqlDataTypes.Int;
                    break;
                case DataverseDataTypes.Lookup:
                    sqlDataType = SqlDataTypes.UniqueIdentifier;
                    break;
                case DataverseDataTypes.Memo:
                    sqlDataType = SqlDataTypes.NText;
                    break;
                case DataverseDataTypes.Money:
                    sqlDataType = SqlDataTypes.Money;
                    break;
                case DataverseDataTypes.Owner:
                    sqlDataType = SqlDataTypes.UniqueIdentifier;
                    break;
                case DataverseDataTypes.Picklist:
                    sqlDataType = SqlDataTypes.Int;
                    break;
                case DataverseDataTypes.State:
                    sqlDataType = SqlDataTypes.Int;
                    break;
                case DataverseDataTypes.Status:
                    sqlDataType = SqlDataTypes.Int;
                    break;
                case DataverseDataTypes.String:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.Uniqueidentifier:
                    sqlDataType = SqlDataTypes.UniqueIdentifier;
                    break;
                case DataverseDataTypes.CalendarRules:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.Virtual:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.BigInt:
                    sqlDataType = SqlDataTypes.BigInt;
                    break;
                case DataverseDataTypes.ManagedProperty:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.EntityName:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.Image:
                    sqlDataType = SqlDataTypes.VarBinaryMax;
                    break;
                case DataverseDataTypes.MultiSelectPicklist:
                    sqlDataType = SqlDataTypes.NVarcharMax;
                    break;
                case DataverseDataTypes.File:
                    sqlDataType = SqlDataTypes.VarBinaryMax;
                    break;
                case DataverseDataTypes.Custom:
                    sqlDataType = null;
                    break;
                default:
                    sqlDataType = null;
                    break;
            }

            return sqlDataType;
        }

        /// <summary>
        /// Normalizes SQL Server data types retrieved from INFORMATION_SCHEMA.COLUMNS.
        /// SQL Server returns abbreviated types (e.g., "varbinary" instead of "varbinary(max)"),
        /// so this method normalizes them to their full equivalents.
        /// </summary>
        internal static string? NormalizeSqlDataType(string? sqlDataType)
        {
            if (string.IsNullOrEmpty(sqlDataType))  
            {
                return sqlDataType;
            }

            var normalizedType = sqlDataType.ToUpperInvariant();

            return normalizedType switch
            {
                "VARBINARY" => SqlDataTypes.VarBinaryMax,
                "DECIMAL" => SqlDataTypes.Decimal,
                "NVARCHAR" => SqlDataTypes.NVarcharMax,
                _ => sqlDataType
            };
        }
    }
}
