namespace Pg.DataverseSync.Engine.Target
{
    public class TargetRecord
    {
        public string ColumnName {  get; private set; }
        public object Value { get; private set; }

        public TargetRecord(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }
    }
}
