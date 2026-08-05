using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    internal class SqlTable
    {
        public string Name { get; }
        public List<Column> Columns { get; }

        public SqlTable(string name)
        {
            Name = name;
            Columns = new List<Column>();
        }

        public SqlTable(string name, List<Column> columns)
        {
            Name = name;
            Columns = columns;
        }
    }
} 
