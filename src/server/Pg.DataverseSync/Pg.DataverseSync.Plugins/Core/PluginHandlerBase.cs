using Microsoft.Xrm.Sdk;

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

        protected bool IsReactivation(IPluginExecutionContext context, Entity target)
        {
            if (!target.Contains("statecode"))
                return false;

            var newState = target.GetAttributeValue<OptionSetValue>("statecode").Value;
            if (newState != 0) //Active
                return false;

            if (!context.PreEntityImages.ContainsKey("PreImage"))
                return false;

            var preImage = context.PreEntityImages["PreImage"];
            if (!preImage.Contains("statecode"))
                return false;

            var oldState = preImage.GetAttributeValue<OptionSetValue>("statecode").Value;
            return oldState == 1; //Inactive
        }

        protected bool IsDeactivation(IPluginExecutionContext context, Entity target)
        {
            if (!target.Contains("statecode"))
                return false;

            var newState = target.GetAttributeValue<OptionSetValue>("statecode").Value;
            if (newState != 1) //Inactive
                return false;

            if (!context.PreEntityImages.ContainsKey("PreImage"))
                return false;

            var preImage = context.PreEntityImages["PreImage"];
            if (!preImage.Contains("statecode"))
                return false;

            var oldState = preImage.GetAttributeValue<OptionSetValue>("statecode").Value;
            return oldState == 0; //Active
        }
    }
}

