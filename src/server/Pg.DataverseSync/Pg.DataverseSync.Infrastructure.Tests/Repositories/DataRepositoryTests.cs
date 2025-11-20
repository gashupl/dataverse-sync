using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Infrastructure.Tests.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pg.DataverseSync.Infrastructure.Tests.Repositories
{ 
    public class DataRepositoryTests
    {
        private readonly IOrganizationServiceFactory _serviceFactory;
        private readonly Guid _ownerId = Guid.NewGuid(); 
        private readonly Guid _goalId = Guid.NewGuid();

        public DataRepositoryTests()
        {
            _serviceFactory = InitDataServiceFactory();
        }
       

        public IOrganizationServiceFactory InitDataServiceFactory()
        {
            var context = new XrmFakedContext(); 
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(pg_synctable));
    
            var record = new pg_synctable
            {
                Id = Guid.NewGuid(),
              
            }; 

            context.Initialize(new List<Entity>() { record });
            return new FakeOrganizationServiceFactory(context.GetFakedOrganizationService());

        }

    }
}

