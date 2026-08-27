using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public static class TableSchemaComparer
    {

        internal static List<Column> GetColumnsToBeRemoved(Table sourceTable, SqlTable targetTable)
        {
            return targetTable.Columns.Where(tc => sourceTable.Columns.All(c => !StringComparer.OrdinalIgnoreCase.Equals(c.Name, tc.Name))).ToList();
        }

        internal static List<Column> GetColumnsToBeAdded(Table sourceTable, SqlTable targetTable)
        {
            var columnsToBeAdded = sourceTable.Columns.Where(c => targetTable.Columns.All(tc => !StringComparer.OrdinalIgnoreCase.Equals(tc.Name, c.Name))).ToList();

            // Convert Column DataTypes to SQL equivalents
            foreach (var column in columnsToBeAdded)
            {
                column.DataType = DataTypesConverter.MapToSqlDataType(column.DataType!);
            }

            return columnsToBeAdded;    
        }

        internal static (List<Column> SourceChanges, List<Column> TargetChanges) GetModifiedColumns(Table sourceTable, SqlTable targetTable)
        {
            var modifiedSourceColumns = new List<Column>();
            var modifiedTargetColumns = new List<Column>();

            foreach (var column in sourceTable.Columns)
            {
                var targetColumn = targetTable.Columns
                    .FirstOrDefault(tc => StringComparer.OrdinalIgnoreCase.Equals(tc.Name, column.Name));

                if (targetColumn != null && 
                    !StringComparer.OrdinalIgnoreCase.Equals(
                        targetColumn.DataType, DataTypesConverter.MapToSqlDataType(column.DataType!)))
                {
                    modifiedSourceColumns.Add(column);
                    modifiedTargetColumns.Add(targetColumn);
                }
            }

            return (modifiedSourceColumns, modifiedTargetColumns);
        }

        internal static List<Column> MergeColumns(List<Column> columnsTable1, List<Column> columnsTable2)
        {
            var mergedColumns = new List<Column>(columnsTable1);

            foreach (var column in columnsTable2)
            {
                if (!mergedColumns
                    .Any(c => c.Name != null && c.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    mergedColumns.Add(column);
                }
            }

            return mergedColumns;
        }

    }
}
