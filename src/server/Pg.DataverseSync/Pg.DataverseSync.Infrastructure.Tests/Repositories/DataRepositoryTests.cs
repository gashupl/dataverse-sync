using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Infrastructure.Repositories;
using Pg.DataverseSync.Infrastructure.Tests.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Pg.DataverseSync.Infrastructure.Tests.Repositories
{ 
    public class DataRepositoryTests
    {
        private readonly IOrganizationServiceFactory _serviceFactory;

        public DataRepositoryTests()
        {
            _serviceFactory = InitDataServiceFactory();
        }

        [Fact]
        public void GetActiveSynchronizedTables_ReturnExpectedResults()
        {
            // Arrange
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(pg_synctable));

            var service = _serviceFactory.CreateOrganizationService(null); 
            var repository = new DataRepository(new FakeOrganizationServiceFactory(service));

            // Act
            var result = repository.GetActiveSynchronizedTables();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("pg_sample1", result[0].pg_name);
        }   


        public IOrganizationServiceFactory InitDataServiceFactory()
        {
            var context = new XrmFakedContext(); 
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(pg_synctable));
    
            var record1 = new pg_synctable
            {
                Id = Guid.NewGuid(),
                pg_name = "pg_sample1",
                StateCode = pg_synctable_statecode.Active
            };

            var record2 = new pg_synctable
            {
                Id = Guid.NewGuid(),
                pg_name = "pg_sample2",
                StateCode = pg_synctable_statecode.Inactive
            };


            context.Initialize(new List<Entity>() { record1, record2 });
            return new FakeOrganizationServiceFactory(context.GetFakedOrganizationService());

        }

    }
}

