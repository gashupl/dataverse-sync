namespace Pg.DataverseSync.Plugins.Core
{
    internal interface IPluginHandler
    {
        void Init(ILocalPluginContext localPluginContext); 
        bool CanExecute(); 
        void Execute(); 
    }
}

