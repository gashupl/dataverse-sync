using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Target.SqlServer.Tests
{
    public class TargetDataStructureServiceTests
    {
        [Fact]
        public void UpsertTable_ThrowsNotImplementedException()
        {
            // Arrange
            var repository = new TargetDataStructureService();
            var table = new Table("test", "Test", false);

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => repository.UpsertTable(table));
        }
    }
}
