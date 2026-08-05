using Pg.DataverseSync.Engine.Core.Model;
using System.Text;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    internal class CreateTableQueryGenerator
    {
        //TODO: Use mapping to map dataverse data types to sql server data types
        internal static string Generate(Table table)
        {
            StringBuilder query = new StringBuilder();
            query.Append($"CREATE TABLE {table.Name} (");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column column = table.Columns[i];
                var dataType = DataTypesConverter.MapToSqlDataType(column.DataType);
                query.Append($"{column.Name} {dataType}");

                if (column.IsIdentity)
                {
                    query.Append(" IDENTITY(1,1)");
                }

                if (!column.IsNullable)
                {
                    query.Append(" NOT NULL");
                }

                if (i < table.Columns.Count - 1)
                {
                    query.Append(", ");
                }
            }

            //Add primary key constraint
            Column? primaryKeyColumn = table.Columns.Find(c => c.IsPrimaryKey);
            if (primaryKeyColumn != null)
            {
                query.Append($", PRIMARY KEY ({primaryKeyColumn.Name})");
            }

            query.Append(")");

            return query.ToString();
        }
    }
}
