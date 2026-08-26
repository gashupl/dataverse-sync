using Pg.DataverseSync.Engine.Core.Model;
using System.Text;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    internal class CreateTableQueryGenerator
    {
        internal static string Generate(Table table)
        {
            StringBuilder query = new StringBuilder();
            query.Append($"CREATE TABLE {table.Name} (");

            var validColumns = table.Columns.Where(c => c != null).ToList();
            for (int i = 0; i < validColumns.Count; i++)
            {
                Column column = validColumns[i];
                var dataType = DataTypesConverter.MapToSqlDataType(column.DataType!);
                query.Append($"{column.Name} {dataType}");

                if (column.IsIdentity)
                {
                    query.Append(" IDENTITY(1,1)");
                }

                if (!column.IsNullable)
                {
                    query.Append(" NOT NULL");
                }

                if (i < validColumns.Count - 1)
                {
                    query.Append(", ");
                }
            }

            //Add primary key constraint
            Column? primaryKeyColumn = table.Columns.Find(c => c != null && c.IsPrimaryKey);
            if (primaryKeyColumn != null)
            {
                query.Append($", PRIMARY KEY ({primaryKeyColumn.Name})");
            }

            query.Append(")");

            return query.ToString();
        }
    }
}
