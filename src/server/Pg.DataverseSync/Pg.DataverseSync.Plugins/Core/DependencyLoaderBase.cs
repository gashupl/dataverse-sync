using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Infrastructure.Repositories;
using System;
using TinyIoC;

namespace Pg.DataverseSync.Plugins.Core
{
    public class DependencyLoaderBase : IDependencyLoader
    {
        public static TinyIoCContainer Container = new TinyIoCContainer(); 

        public void RegisterDefaults(LocalPluginContext localContext)
        {
            localContext.Trace("Registering default dependencies...");
            var userOrganizationService = localContext.OrgSvcFactory.CreateOrganizationService(Guid.Empty);
            var dataRepository = new DataRepository(localContext.OrgSvcFactory);

            Container.Register(localContext);
            Container.Register(localContext.Logger);
            Container.Register(localContext.OrgSvcFactory);
            Container.Register(userOrganizationService);
            Container.Register(localContext.PluginUserService);
            Container.Register(localContext.TracingService);
            Container.Register<IRepository>(dataRepository); 
        }

        public I Get<I>() where I : class
        {
            try
            {
                return Container.Resolve<I>();
            }
            catch (TinyIoCResolutionException ex)
            {
                throw new InvalidPluginExecutionException(ex.InnerException?.Message ?? "An error occurred while resolving the dependency.", ex);
            }
            
        }

        public void Register<I, C>() where I : class where C : class, I
        {
            Container.Register<I, C>();
        }
    }
}

