namespace Pg.DataverseSync.Plugins.Core
{
    public abstract class PluginHandlerBase : IPluginHandler
    {
        protected ILocalPluginContext localPluginContext { get; set; }

        public void Init(ILocalPluginContext localPluginContext)
        {
            this.localPluginContext = localPluginContext; 
        }

        public abstract bool CanExecute();

        public abstract void Execute(); 
    }
}

