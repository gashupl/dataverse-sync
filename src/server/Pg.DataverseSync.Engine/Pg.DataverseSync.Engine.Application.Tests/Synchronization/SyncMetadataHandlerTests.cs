using Pg.DataverseSync.Engine.Application.Synchronization;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    public class SyncMetadataHandlerTests
    {
        [Fact]
        public void Handle_InvokesCurrentAndNextHandlerInOrder()
        {
            // Arrange
            var callOrder = new List<int>();
            var first = new TestSyncMetadataHandler(_ => callOrder.Add(1));
            var second = new TestSyncMetadataHandler(_ => callOrder.Add(2));
            first.SetNext(second);
            var context = new SyncMetadataExecutionContext();

            // Act
            first.Handle(context);

            // Assert
            Assert.Equal(new List<int> { 1, 2 }, callOrder);
        }

        [Fact]
        public void Handle_WhenCurrentHandlerThrows_DoesNotInvokeNextHandler()
        {
            // Arrange
            var nextInvoked = false;
            var first = new TestSyncMetadataHandler(_ => throw new InvalidOperationException("failure"));
            var second = new TestSyncMetadataHandler(_ => nextInvoked = true);
            first.SetNext(second);
            var context = new SyncMetadataExecutionContext();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => first.Handle(context));
            Assert.False(nextInvoked);
        }
    }
}
