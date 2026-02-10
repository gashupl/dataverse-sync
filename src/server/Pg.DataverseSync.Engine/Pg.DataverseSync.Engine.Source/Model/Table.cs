namespace Pg.DataverseSync.Engine.Source.Model
{
    public class Table
    {

        public string Name { get; }

        public string DisplayName { get; }

        public bool IsActivity { get; }

        public List<Column> Columns { get; }

        public Table(string name, string displayName, bool isActivity)
        {            
            Name = name;
            DisplayName = displayName;
            IsActivity = isActivity;
            Columns = new List<Column>();
        }

    }
}
