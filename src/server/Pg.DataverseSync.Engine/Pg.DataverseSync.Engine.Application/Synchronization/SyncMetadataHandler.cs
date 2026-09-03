namespace Pg.DataverseSync.Engine.Application.Synchronization
{
    internal abstract class SyncMetadataHandler
    {
        private SyncMetadataHandler? _next;

        public SyncMetadataHandler SetNext(SyncMetadataHandler next)
        {
            _next = next;
            return next;
        }

        public void Handle(SyncMetadataExecutionContext context)
        {
            Execute(context);
            if (_next != null)
            {
                _next.Handle(context);
            }
        }

        protected abstract void Execute(SyncMetadataExecutionContext context);
    }
}
