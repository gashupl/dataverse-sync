using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Pg.DataverseSync.Infrastructure.Core;
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

        [Fact]
        public void GetTablesFromMetadata_ReturnsExpectedTables()
        {
            var context = new XrmFakedContext();
            var service = context.GetFakedOrganizationService();

            // Mock the Execute method for RetrieveAllEntitiesRequest
            context.AddExecutionMock<RetrieveAllEntitiesRequest>(req =>
            {
                var response = new RetrieveAllEntitiesResponse
                {
                    Results = new ParameterCollection
                    {
                        ["EntityMetadata"] = new[]
                        {
                    new Microsoft.Xrm.Sdk.Metadata.EntityMetadata
                    {
                        LogicalName = "pg_standardtable",
                        DisplayName = new Label("Standard Table", 1033),
                        TableType = TableTypes.Standard
                    },
                    new Microsoft.Xrm.Sdk.Metadata.EntityMetadata
                    {
                        LogicalName = "pg_sampleelastictable",
                        DisplayName = new Label("Elastic Table", 1033),
                        TableType = TableTypes.Elastic
                    }
                }
                    }
                };
                return response;
            });

            var repository = new DataRepository(new FakeOrganizationServiceFactory(service));
            var tables = repository.GetStandardTablesFromMetadata();

            Assert.NotNull(tables);
            Assert.Single(tables);
            Assert.Contains(tables, t => t.SchemaName == "pg_standardtable");
        }


        private IOrganizationServiceFactory InitDataServiceFactory()
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

