using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Core.Tests.Model
{
    public class TableTests
    {
        [Fact]
        public void GetChecksum_ReturnsSameValue_ForEquivalentTables_WhenColumnsAreInDifferentOrder()
        {
            var firstTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("name", "nvarchar", isNullable: false),
                    new("accountid", "uniqueidentifier", isPrimaryKey: true, isIdentity: true, isNullable: false)
                }
            };

            var secondTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("accountid", "uniqueidentifier", isPrimaryKey: true, isIdentity: true, isNullable: false),
                    new("name", "nvarchar", isNullable: false)
                }
            };

            var firstChecksum = firstTable.GetChecksum();
            var secondChecksum = secondTable.GetChecksum();

            Assert.Equal(firstChecksum, secondChecksum);
        }

        [Fact]
        public void GetChecksum_ReturnsDifferentValue_WhenTableStateDiffers()
        {
            var firstTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("name", "nvarchar", isNullable: false)
                }
            };

            var secondTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("name", "nvarchar(max)", isNullable: false)
                }
            };

            var firstChecksum = firstTable.GetChecksum();
            var secondChecksum = secondTable.GetChecksum();

            Assert.NotEqual(firstChecksum, secondChecksum);
        }

        [Fact]
        public void GetChecksum_ReturnsExpectedValue_ForEquivalentTables_WhenColumnsAreInDifferentOrder()
        {
            const string expectedChecksum = "31038F62ADC1894DF3014DE671C7374E019B375335527C9A2B5BD299E89AE21E";

            var firstTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("name", "nvarchar", isNullable: false),
                    new("accountid", "uniqueidentifier", isPrimaryKey: true, isIdentity: true, isNullable: false)
                }
            };

            var secondTable = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("accountid", "uniqueidentifier", isPrimaryKey: true, isIdentity: true, isNullable: false),
                    new("name", "nvarchar", isNullable: false)
                }
            };

            var firstChecksum = firstTable.GetChecksum();
            var secondChecksum = secondTable.GetChecksum();

            Assert.Equal(expectedChecksum, firstChecksum);
            Assert.Equal(expectedChecksum, secondChecksum);
        }

        [Fact]
        public void GetChecksum_ReturnsExpectedValue_WhenTableStateMatchesKnownChecksum()
        {
            const string expectedChecksum = "E37EA9855EDDC8F5EBF88507DB4451B4B565A18032AF2A7F4340B48D30135C46";

            var table = new Table("account", "Account", false)
            {
                Columns = new List<Column>
                {
                    new("name", "nvarchar(max)", isNullable: false)
                }
            };

            var checksum = table.GetChecksum();

            Assert.Equal(expectedChecksum, checksum);
        }
    }
}
