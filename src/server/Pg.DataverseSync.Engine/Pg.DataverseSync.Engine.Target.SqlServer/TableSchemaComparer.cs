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
            return sourceTable.Columns
                .Where(c => targetTable.Columns.All(tc => !StringComparer.OrdinalIgnoreCase.Equals(tc.Name, c.Name)))
                .Select(c => new Column(c.Name, DataTypesConverter.MapToSqlDataType(c.DataType!), c.IsPrimaryKey, c.IsNullable, c.IsIdentity))
                .ToList();
        }

        internal static (List<Column> SourceChanges, List<Column> TargetChanges) GetModifiedColumns(Table sourceTable, SqlTable targetTable)
        {
            var modifiedColumns = sourceTable.Columns
                .Select(column => new
                {
                    SourceColumn = column,
                    TargetColumn = targetTable.Columns.FirstOrDefault(tc => StringComparer.OrdinalIgnoreCase.Equals(tc.Name, column.Name))
                })
                .Where(x => x.TargetColumn != null && 
                    !StringComparer.OrdinalIgnoreCase.Equals(
                        x.TargetColumn.DataType, DataTypesConverter.MapToSqlDataType(x.SourceColumn.DataType!)))
                .ToList();

            return (
                modifiedColumns.Select(x => x.SourceColumn).ToList(),
                modifiedColumns.Select(x => x.TargetColumn!).ToList()
            );
        }

        internal static List<Column> MergeColumns(List<Column> columnsTable1, List<Column> columnsTable2)
        {
            var mergedColumns = new List<Column>(columnsTable1);
            var columnToAdd = columnsTable2
                .Where(column => !mergedColumns
                    .Any(c => c.Name != null && c.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            mergedColumns.AddRange(columnToAdd);
            return mergedColumns;
        }

    }
}
