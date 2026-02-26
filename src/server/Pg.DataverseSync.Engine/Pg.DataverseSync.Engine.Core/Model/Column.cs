namespace Pg.DataverseSync.Engine.Core.Model
{
    public class Column
    {
        public string Name { get; }
        public string? DataType { get; }
        public bool IsPrimaryKey { get; }
        public bool IsIdentity { get; }
        public bool IsNullable { get; }

        public Column(string name, string? dataType, bool isPrimaryKey = false, bool isIdentity = false, bool isNullable = true)
        {
            Name = name;
            DataType = dataType;
            IsPrimaryKey = isPrimaryKey;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
        }
    }
}
