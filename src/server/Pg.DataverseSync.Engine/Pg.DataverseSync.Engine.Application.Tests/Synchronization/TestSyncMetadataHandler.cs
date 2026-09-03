using Pg.DataverseSync.Engine.Application.Synchronization;

namespace Pg.DataverseSync.Engine.Application.Tests.Synchronization
{
    internal sealed class TestSyncMetadataHandler : SyncMetadataHandler
    {
        private readonly Action<SyncMetadataExecutionContext> _executeAction;

        public TestSyncMetadataHandler(Action<SyncMetadataExecutionContext> executeAction)
        {
            _executeAction = executeAction;
        }

        protected override void Execute(SyncMetadataExecutionContext context)
        {
            _executeAction(context);
        }
    }
}
