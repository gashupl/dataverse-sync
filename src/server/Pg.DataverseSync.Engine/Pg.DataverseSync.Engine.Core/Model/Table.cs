using System.Security.Cryptography;
using System.Text;

namespace Pg.DataverseSync.Engine.Core.Model
{
    public class Table : IChecksumSource
    {
        public string Name { get; }

        public string DisplayName { get; }

        public bool IsActivity { get; }

        public List<Column> Columns { get; set; }

        public Table(string name, string displayName, bool isActivity)
        {
            Name = name;
            DisplayName = displayName;
            IsActivity = isActivity;
            Columns = new List<Column>();
        }

        public string GetChecksum()
        {
            var builder = new StringBuilder();

            builder.Append("Table|");
            builder.Append(Name).Append('|');
            builder.Append(DisplayName).Append('|');
            builder.Append(IsActivity).Append('|');

            foreach (var column in Columns
                .OrderBy(c => c.Name, StringComparer.Ordinal)
                .ThenBy(c => c.DataType, StringComparer.Ordinal)
                .ThenBy(c => c.IsPrimaryKey)
                .ThenBy(c => c.IsIdentity)
                .ThenBy(c => c.IsNullable))
            {
                builder.Append("Column|");
                builder.Append(column.Name ?? string.Empty).Append('|');
                builder.Append(column.DataType ?? string.Empty).Append('|');
                builder.Append(column.IsPrimaryKey).Append('|');
                builder.Append(column.IsIdentity).Append('|');
                builder.Append(column.IsNullable).Append('|');
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
