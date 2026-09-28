namespace Pg.DataverseSync.Engine.Target
{
    public class TargetColumn
    {
        public string ColumnName {  get; private set; }
        public object Value { get; private set; }
        public bool IsPrimaryKey { get; private set; } = false;
        public TargetColumn(string columnName, object value, bool isPrimaryKey = false)
        {
            ColumnName = columnName;
            Value = value;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
