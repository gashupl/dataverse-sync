namespace Pg.DataverseSync.Engine.Target
{
    public class TargetRecord
    {
        public string TableName { get; set; }
        public List<TargetColumn> Columns { get; private set; }

        public TargetRecord(string tableName) 
        {
            TableName = tableName;
            Columns = new List<TargetColumn>();
        }

        public void AddColumn(string columnName, object value, bool isPrimaryKey = false)
        {
            if(isPrimaryKey && Columns.Exists(c => c.IsPrimaryKey))
            {
                throw new InvalidOperationException("A primary key column already exists for this record.");
            }
            
            if(Columns.Exists(c => c.ColumnName == columnName))
            {
                throw new InvalidOperationException($"A column with the name '{columnName}' already exists for this record.");
            }

            Columns.Add(new TargetColumn(columnName, value, isPrimaryKey));
        }
    }
}
